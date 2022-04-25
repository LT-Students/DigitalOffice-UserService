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

    #region Predicates

    private IQueryable<DbUser> CreateGetPredicates(
      GetUserFilter filter,
      IQueryable<DbUser> dbUsers)
    {
      if (filter.UserId.HasValue)
      {
        dbUsers = dbUsers.Where(u => u.Id == filter.UserId);
      }
      else if (!string.IsNullOrEmpty(filter.Login))
      {
        dbUsers = dbUsers
          .Include(u => u.Credentials)
          .Where(u => u.Credentials.Login == filter.Login);
      }

      if (filter.IncludeCommunications)
      {
        dbUsers = dbUsers.Include(u => u.Communications);

        if (!string.IsNullOrEmpty(filter.Email?.Trim()))
        {
          dbUsers = dbUsers
            .Where(u => u.Communications.Any(c => c.IsConfirmed && c.Value == filter.Email));
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
      dbUsers = dbUsers.Include(u => u.Pending);

      return dbUsers;
    }

    private IQueryable<DbUser> CreateFindPredicates(
      FindUsersFilter filter,
      List<Guid> userIds,
      IQueryable<DbUser> dbUsers)
    {
      if (userIds is not null && userIds.Any())
      {
        dbUsers = dbUsers.Where(u => userIds.Contains(u.Id));
      }

      if (filter.Active.HasValue)
      {
        dbUsers = filter.Active.Value
          ? dbUsers.Where(u => u.IsActive)
          : dbUsers.Where(u => !u.IsActive);
      }

      if (filter.IncludeCurrentAvatar)
      {
        dbUsers = dbUsers.Include(u => u.Avatars.Where(ua => ua.IsCurrentAvatar));
      }

      if (!string.IsNullOrEmpty(filter.FullNameIncludeSubstring))
      {
        dbUsers = dbUsers.Where(
          u =>
            u.FirstName.Contains(filter.FullNameIncludeSubstring)
            || u.LastName.Contains(filter.FullNameIncludeSubstring)
            || u.MiddleName.Contains(filter.FullNameIncludeSubstring));
      }

      if (filter.AscendingSort.HasValue)
      {
        dbUsers = filter.AscendingSort.Value
          ? dbUsers.OrderBy(u => u.LastName).ThenBy(u => u.LastName).ThenBy(u => u.MiddleName)
          : dbUsers.OrderByDescending(u => u.LastName).ThenByDescending(u => u.LastName).ThenByDescending(u => u.MiddleName);
      }

      return dbUsers;
    }

    #endregion

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

    public async Task<DbUser> GetAsync(Guid userId, bool includeBaseEmail = false)
    {
      IQueryable<DbUser> dbUser = _provider.Users.AsQueryable();

      if (includeBaseEmail)
      {
        dbUser = dbUser
          .Include(u => u.Communications.FirstOrDefault(c => c.Type == (int)CommunicationType.BaseEmail));
      }

      return await dbUser.FirstOrDefaultAsync(u => u.Id == userId);
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
        .Where(x => usersIds.Contains(x.Id) && x.IsActive == true)
        .ToListAsync();
    }

    public async Task<List<Guid>> AreExistingIdsAsync(List<Guid> usersIds)
    {
      if (usersIds is null)
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

    public async Task<bool> SwitchActiveStatusAsync(Guid userId, bool isActive)
    {
      DbUser dbUser = await _provider.Users.Include(x => x.Credentials)
        .FirstOrDefaultAsync(u => u.Id == userId);

      if (dbUser is null || dbUser.Credentials is null)
      {
        return false;
      }

      dbUser.IsActive = isActive;
      dbUser.ModifiedBy = _httpContextAccessor.HttpContext.Items.ContainsKey(ConstStrings.UserId) ?
        _httpContextAccessor.HttpContext.GetUserId() :
        null;
      dbUser.ModifiedAtUtc = DateTime.UtcNow;
      dbUser.Credentials.IsActive = dbUser.IsActive;

      await _provider.SaveAsync();

      return true;
    }

    public async Task<(List<DbUser> dbUsers, int totalCount)> FindAsync(FindUsersFilter filter, List<Guid> userIds = null)
    {
      if (filter is null)
      {
        return (null, default);
      }

      IQueryable<DbUser> dbUsers = CreateFindPredicates(
        filter,
        userIds,
        _provider.Users.AsQueryable());

      return (
        await dbUsers.Skip(filter.SkipCount).Take(filter.TakeCount).ToListAsync(),
        await dbUsers.CountAsync());
    }

    public async Task<bool> DoesExistAsync(Guid userId)
    {
      return await _provider.Users
        .FirstOrDefaultAsync(u => u.Id == userId) != null;
    }

    public async Task<List<DbUser>> SearchAsync(string text)
    {
      return await _provider.Users
        .Where(
          u =>
            u.FirstName.Contains(text)
            || u.LastName.Contains(text)
            || u.MiddleName.Contains(text))
        .ToListAsync();
    }
  }
}
