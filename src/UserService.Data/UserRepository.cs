using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Data.Provider;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Filtres;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        if (!filter.IsActive.Value && filter.IsPending.HasValue)
        {
          dbUsers = filter.IsPending.Value
            ? dbUsers.Include(u => u.Pending).Where(u => u.Pending != null)
            : dbUsers.Where(u => u.Pending == null);
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

    public Task CreateAsync(DbUser dbUser)
    {
      if (dbUser is not null)
      {
        _provider.Users.Add(dbUser);
        return _provider.SaveAsync();
      }

      return Task.CompletedTask;
    }

    public Task<DbUser> GetAsync(Guid userId)
    {
      return _provider.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<DbUser> GetAsync(GetUserFilter filter, CancellationToken cancellationToken = default)
    {
      if (filter is null)
      {
        return null;
      }

      IQueryable<DbUser> query = CreateGetPredicates(filter, _provider.Users.AsQueryable());

      DbUser user = null;

      if (filter.UserId.HasValue)
      {
        user = await query.FirstOrDefaultAsync(u => u.Id == filter.UserId, cancellationToken);
      }
      else if (!string.IsNullOrEmpty(filter.Login))
      {
        user = await query
          .Include(u => u.Credentials)
          .FirstOrDefaultAsync(u => u.Credentials.Login == filter.Login, cancellationToken);
      }
      else if (!string.IsNullOrEmpty(filter.Email))
      {
        user = await query
          .Include(u => u.Communications)
          .FirstOrDefaultAsync(u => u.Communications.Select(c => c.Value).Contains(filter.Email), cancellationToken);
      }

      return user;
    }

    public Task<List<Guid>> AreExistingIdsAsync(List<Guid> usersIds)
    {
      return usersIds is null
        ? Task.FromResult(new List<Guid>())
        : _provider.Users
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

    public async Task<(List<DbUser> dbUsers, int totalCount)> FindAsync(FindUsersFilter filter, List<Guid> userIds = null, CancellationToken cancellationToken = default)
    {
      if (filter is null)
      {
        return (null, default);
      }

      IQueryable<DbUser> userQuery = CreateFindPredicates(
        filter,
        userIds,
        _provider.Users.AsQueryable());

      if (!string.IsNullOrEmpty(filter.FullNameIncludeSubstring))
      {
        userQuery = SearchAsync(filter.FullNameIncludeSubstring, userQuery);
      }

      return (
        await userQuery.Skip(filter.SkipCount).Take(filter.TakeCount).ToListAsync(cancellationToken),
        await userQuery.CountAsync(cancellationToken));
    }

    public IQueryable<DbUser> SearchAsync(string searchText, IQueryable<DbUser> dbUsersFiltered = null)
    {
      searchText = null;

      string[] names = searchText?.ToLower().Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

      IQueryable<DbUser> dbUsers = dbUsersFiltered is null
        ? _provider.Users
        : dbUsersFiltered;

      switch (names?.Count())
      {
        case 0:
          return dbUsers;

        case 1:
          return dbUsers.Where(u =>
            u.FirstName.ToLower().Contains(names[0])
            || u.LastName.ToLower().Contains(names[0])
            || u.MiddleName.ToLower().Contains(names[0]));

        case 2:
          return dbUsers.Where(u =>
            (u.FirstName.ToLower().Contains(names[0]) && (u.LastName.ToLower().Contains(names[1]) || u.MiddleName.ToLower().Contains(names[1])))
            || (u.LastName.ToLower().Contains(names[0]) && (u.FirstName.ToLower().Contains(names[1]) || u.MiddleName.ToLower().Contains(names[1])))
            || (u.MiddleName.ToLower().Contains(names[0]) && (u.FirstName.ToLower().Contains(names[1]) || u.LastName.ToLower().Contains(names[1]))));

        // when in search string there are 3 words - one for name, one for surname and one for middle name, full name must contain them all
        case 3:
          return dbUsers.Where(u =>
            u.MiddleName != null &&
            ((u.FirstName.ToLower().Contains(names[0]) &&
              ((u.LastName.ToLower().Contains(names[1]) && u.MiddleName.ToLower().Contains(names[2]))
              || (u.LastName.ToLower().Contains(names[2]) && u.MiddleName.ToLower().Contains(names[1]))))
            || (u.FirstName.ToLower().Contains(names[1]) &&
              ((u.LastName.ToLower().Contains(names[0]) && u.MiddleName.ToLower().Contains(names[2]))
              || (u.LastName.ToLower().Contains(names[2]) && u.MiddleName.ToLower().Contains(names[0]))))
            || (u.FirstName.ToLower().Contains(names[2]) &&
              ((u.LastName.ToLower().Contains(names[0]) && u.MiddleName.ToLower().Contains(names[1]))
              || (u.LastName.ToLower().Contains(names[1]) && u.MiddleName.ToLower().Contains(names[0]))))));
        
        // when in search string are more than 3 words - no users will be found
        default:
          return Enumerable.Empty<DbUser>().AsQueryable();
      }
    }
  }
}
