using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
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
      IQueryable<DbUser> query)
    {
      if (filter.IncludeCommunications)
      {
        query = query.Include(u => u.Communications);
      }

      if (filter.IncludeAvatars)
      {
        query = query.Include(u => u.Avatars);
      }
      else if (filter.IncludeCurrentAvatar)
      {
        query = query.Include(u => u.Avatars.Where(ua => ua.IsCurrentAvatar));
      }

      query = query
        .Include(u => u.Pending)
        .Include(u => u.Addition).ThenInclude(ua => ua.Gender);

      return query;
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

      if (filter.IsActive.HasValue)
      {
        dbUsers = dbUsers.Where(u => u.IsActive == filter.IsActive.Value);

        if (!filter.IsActive.Value)
        {
          dbUsers = dbUsers.Include(u => u.Pending);
        }
      }

      if (filter.IncludeCurrentAvatar)
      {
        dbUsers = dbUsers.Include(u => u.Avatars.Where(ua => ua.IsCurrentAvatar));
      }

      if (filter.IsAscendingSort.HasValue)
      {
        dbUsers = filter.IsAscendingSort.Value
          ? dbUsers.OrderBy(u => u.LastName).ThenBy(u => u.LastName).ThenBy(u => u.MiddleName)
          : dbUsers.OrderByDescending(u => u.LastName).ThenByDescending(u => u.LastName).ThenByDescending(u => u.MiddleName);
      }

      if (filter.IncludeCommunications)
      {
        dbUsers = dbUsers.Include(u => u.Communications);
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

    public Task<DbUser> GetAsync(Guid userId)
    {
      return _provider.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<DbUser> GetAsync(GetUserFilter filter)
    {
      if (filter is null)
      {
        return null;
      }

      IQueryable<DbUser> query = CreateGetPredicates(filter, _provider.Users.AsQueryable());

      DbUser user = null;

      if (filter.UserId.HasValue)
      {
        user = await query.FirstOrDefaultAsync(u => u.Id == filter.UserId);
      }
      else if (!string.IsNullOrEmpty(filter.Login))
      {
        user = await query
          .Include(u => u.Credentials)
          .FirstOrDefaultAsync(u => u.Credentials.Login == filter.Login);
      }
      else if (!string.IsNullOrEmpty(filter.Email))
      {
        user = await query
          .Include(u => u.Communications)
          .FirstOrDefaultAsync(u => u.Communications.Select(c => c.Value).Contains(filter.Email));
      }

      return user;
    }

    public async Task<List<Guid>> AreExistingIdsAsync(List<Guid> usersIds)
    {
      return usersIds is null
        ? new()
        : await _provider.Users
          .Where(u => usersIds.Contains(u.Id) && u.IsActive)
          .Select(u => u.Id)
          .ToListAsync();
    }

    public async Task<bool> EditUserAdditionAsync(Guid userId, JsonPatchDocument<DbUserAddition> patch)
    {
      DbUserAddition dbUserAddition = await _provider.UsersAdditions
        .FirstOrDefaultAsync(x => x.UserId == userId);

      if (patch is null || dbUserAddition is null)
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
      DbUser dbUser = await _provider.Users
        .FirstOrDefaultAsync(x => x.Id == userId);

      if (patch is null || dbUser is null)
      {
        return false;
      }

      patch.ApplyTo(dbUser);
      dbUser.CreatedBy = _httpContextAccessor.HttpContext.GetUserId();
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
      dbUser.CreatedBy = _httpContextAccessor.HttpContext.Items.ContainsKey(ConstStrings.UserId) ?
        _httpContextAccessor.HttpContext.GetUserId() :
        userId;
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

      if (!string.IsNullOrEmpty(filter.FullNameIncludeSubstring))
      {
        dbUsers = SearchAsync(filter.FullNameIncludeSubstring, dbUsers);
      }

      return (
        await dbUsers.Skip(filter.SkipCount).Take(filter.TakeCount).ToListAsync(),
        dbUsers.Count());
    }

    public IQueryable<DbUser> SearchAsync(string text, IQueryable<DbUser> dbUsersFiltered = null)
    {
      string[] cleanedFullName = string.Join(" ", text.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
        .ToLower().Split(" ");

      IQueryable<DbUser> dbUsers = dbUsersFiltered is null && !dbUsersFiltered.Any() ? _provider.Users : dbUsersFiltered;

      if (cleanedFullName.Count() == 1)
      {
        return dbUsers.Where(u =>
            u.FirstName.ToLower().Contains(cleanedFullName[0])
            || u.LastName.ToLower().Contains(cleanedFullName[0])
            || u.MiddleName.ToLower().Contains(cleanedFullName[0]));
      }

      if (cleanedFullName.Count() == 2)
      {
        return dbUsers.Where(u =>
            u.FirstName.ToLower().Contains(cleanedFullName[0]) || u.FirstName.ToLower().Contains(cleanedFullName[1])
            || u.LastName.ToLower().Contains(cleanedFullName[0]) || u.LastName.ToLower().Contains(cleanedFullName[1])
            || u.MiddleName.ToLower().Contains(cleanedFullName[0]) || u.MiddleName.ToLower().Contains(cleanedFullName[1]));
      }

      return dbUsers.Where(u =>
        u.FirstName.ToLower().Contains(cleanedFullName[0]) || u.FirstName.ToLower().Contains(cleanedFullName[1]) || u.FirstName.ToLower().Contains(cleanedFullName[2])
        && u.LastName.ToLower().Contains(cleanedFullName[0]) || u.LastName.ToLower().Contains(cleanedFullName[1]) || u.LastName.ToLower().Contains(cleanedFullName[2])
        && u.MiddleName.ToLower().Contains(cleanedFullName[0]) || u.MiddleName.ToLower().Contains(cleanedFullName[1]) || u.MiddleName.ToLower().Contains(cleanedFullName[2]));
    }
  }
}
