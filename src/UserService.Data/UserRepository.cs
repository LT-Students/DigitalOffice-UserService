using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Filtres;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data
{
  public class UserRepository : IUserRepository
  {
    private readonly IDataProvider _provider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private IQueryable<DbUser> CreateGetPredicates(
      GetUserFilter filter,
      IQueryable<DbUser> dbUsers)
    {
      if (filter.UserId.HasValue)
      {
        dbUsers = dbUsers.Where(u => u.Id == filter.UserId);
      }

      if (!string.IsNullOrEmpty(filter.Name?.Trim()))
      {
        dbUsers = dbUsers
          .Where(u => u.FirstName.Contains(filter.Name) || u.LastName.Contains(filter.Name));
      }

      if (filter.IncludeCommunications)
      {
        dbUsers = dbUsers.Include(u => u.Communications);

        if (!string.IsNullOrEmpty(filter.Email?.Trim()))
        {
          dbUsers = dbUsers
            .Where(u => u.Communications
              .Any(c => c.Type == (int)CommunicationType.Email && c.Value == filter.Email));
        }
      }

      if (filter.IncludeAvatars)
      {
        dbUsers = dbUsers.Include(u => u.Avatars);
      }
      else if (filter.IncludeCurrentAvatar)
      {
        dbUsers = dbUsers.Include(u => u.Avatars.Where(ua => ua.IsCurrentAvatar));
      }

      dbUsers = dbUsers.Include(u => u.Addition).ThenInclude(ua => ua.Gender);

      return dbUsers;
    }

    public UserRepository(
      IDataProvider provider,
      IHttpContextAccessor httpContextAccessor)
    {
      _provider = provider;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Guid> CreateAsync(DbUser dbUser)
    {
      if (dbUser is null)
      {
        return default;
      }

      _provider.Users.Add(dbUser);
      await _provider.SaveAsync();

      return dbUser.Id;
    }

    public async Task CreatePendingAsync(DbPendingUser dbPendingUser)
    {
      _provider.PendingUsers.Add(dbPendingUser);
      await _provider.SaveAsync();
    }

    public async Task<DbUser> GetAsync(Guid id)
    {
      return await GetAsync(new GetUserFilter() { UserId = id });
    }

    public async Task<DbUser> GetAsync(GetUserFilter filter)
    {
      if (filter is null)
      {
        return null;
      }

      IQueryable<DbUser> dbUsers = _provider.Users.AsQueryable();

      return await CreateGetPredicates(filter, dbUsers)
        .FirstOrDefaultAsync();
    }

    public async Task<List<DbUser>> GetAsync(List<Guid> usersIds, bool includeAvatars = false)
    {
      if (usersIds is null)
      {
        return null;
      }

      IQueryable<DbUser> dbUsers = _provider.Users.AsQueryable();

      if (includeAvatars)
      {
        dbUsers = dbUsers.Include(u => u.Avatars);
      }

      return await dbUsers
        .Where(x => usersIds.Contains(x.Id))
        .ToListAsync();
    }

    public async Task<List<Guid>> AreExistingIdsAsync(List<Guid> usersIds)
    {
      if (usersIds == null)
      {
        return null;
      }

      return await _provider.Users
        .Where(u => usersIds.Contains(u.Id) && u.IsActive)
        .Select(u => u.Id)
        .ToListAsync();
    }

    public async Task<bool> EditUserAdditionAsync(Guid userId, JsonPatchDocument<DbUserAddition> patch)
    {
      if (patch is null)
      {
        return false;
      }

      DbUserAddition dbUserAddition = await _provider.UsersAdditions
        .FirstOrDefaultAsync(x => x.UserId == userId);

      if (dbUserAddition == default)
      {
        return false;
      }

      patch.ApplyTo(dbUserAddition);
      dbUserAddition.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
      dbUserAddition.ModifiedAtUtc = DateTime.UtcNow;
      await _provider.SaveAsync();

      return true;
    }

    public async Task<bool> EditUserAsync(Guid userId, JsonPatchDocument<DbUser> patch)
    {
      if (patch is null)
      {
        return false;
      }

      DbUser dbUser = await _provider.Users
        .FirstOrDefaultAsync(x => x.Id == userId);

      if (dbUser == default)
      {
        return false;
      }

      patch.ApplyTo(dbUser);
      dbUser.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
      dbUser.ModifiedAtUtc = DateTime.UtcNow;
      await _provider.SaveAsync();

      return true;
    }

    /// <inheritdoc />
    public async Task<bool> SwitchActiveStatusAsync(Guid userId, bool status)
    {
      DbUser dbUser = await _provider.Users
        .FirstOrDefaultAsync(u => u.Id == userId);

      if (dbUser == null)
      {
        return false;
      }

      dbUser.IsActive = status;

      _provider.Users.Update(dbUser);
      dbUser.ModifiedBy = _httpContextAccessor.HttpContext.Items.ContainsKey(ConstStrings.UserId) ?
        _httpContextAccessor.HttpContext.GetUserId() :
        null;
      dbUser.ModifiedAtUtc = DateTime.UtcNow;
      await _provider.SaveAsync();

      return true;
    }

    public async Task<(List<DbUser> dbUsers, int totalCount)> FindAsync(FindUsersFilter filter)
    {
      if (filter == null)
      {
        return (null, default);
      }

      IQueryable<DbUser> dbUsers = _provider.Users.AsQueryable();

      if (!filter.IncludeDeactivated)
      {
        dbUsers = dbUsers.Where(u => u.IsActive);
      }

      if (filter.IncludeCurrentAvatar)
      {
        dbUsers = dbUsers.Include(u => u.Avatars.Where(ua => ua.IsCurrentAvatar));
      }

      return (
        await dbUsers.Skip(filter.SkipCount).Take(filter.TakeCount).ToListAsync(),
        await dbUsers.CountAsync());
    }

    public async Task<DbPendingUser> GetPendingUserAsync(Guid userId)
    {
      return await _provider.PendingUsers
        .FirstOrDefaultAsync(pu => pu.UserId == userId);
    }

    public async Task DeletePendingUserAsync(Guid userId)
    {
      DbPendingUser dbPendingUser = await _provider.PendingUsers
        .FirstOrDefaultAsync(pu => pu.UserId == userId);

      if (dbPendingUser != null)
      {
        _provider.PendingUsers.Remove(dbPendingUser);
        await _provider.SaveAsync();
      }
    }

    public async Task<bool> IsUserExistAsync(Guid userId)
    {
      return await _provider.Users
        .FirstOrDefaultAsync(u => u.Id == userId) != null;
    }

    public async Task<List<DbUser>> SearchAsync(string text)
    {
      return await _provider.Users
        .Where(u => string.Join(" ", u.FirstName, u.MiddleName, u.LastName)
        .Contains(text))
        .ToListAsync();
    }

    public async Task<bool> PendingUserExistAsync(Guid userId)
    {
      return await _provider.PendingUsers.AnyAsync(pu => pu.UserId == userId);
    }
  }
}
