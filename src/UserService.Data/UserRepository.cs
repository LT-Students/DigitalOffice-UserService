using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Exceptions.Models;
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
  /// <inheritdoc/>
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

      if (filter.IncludeCertificates)
      {
        dbUsers = dbUsers.Include(u => u.Certificates);
      }

      if (filter.IncludeEducations)
      {
        dbUsers = dbUsers.Include(u => u.Educations.Where(e => e.IsActive));
      }

      if (filter.IncludeAchievements)
      {
        dbUsers = dbUsers.Include(u => u.Achievements).ThenInclude(a => a.Achievement);
      }

      if (filter.IncludeSkills)
      {
        dbUsers = dbUsers.Include(u => u.Skills).ThenInclude(s => s.Skill);
      }

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
      if (dbUser == null)
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
      GetUserFilter filter = new()
      {
        UserId = id
      };

      return await GetAsync(filter);
    }

    public async Task<DbUser> GetAsync(GetUserFilter filter)
    {
      if (filter == null)
      {
        return null;
      }

      IQueryable<DbUser> dbUsers = _provider.Users.AsQueryable();

      return await CreateGetPredicates(filter, dbUsers)
        .FirstOrDefaultAsync();
    }

    public async Task<List<DbUser>> GetAsync(List<Guid> userIds)
    {
      if (userIds == null)
      {
        return null;
      }

      return await _provider.Users
        .Where(x => userIds.Contains(x.Id))
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

    public async Task<bool> EditUserAsync(Guid userId, JsonPatchDocument<DbUser> patch)
    {
      if (patch == null)
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

    //remove to education service
    public DbSkill FindSkillByName(string name)
    {
      return _provider.Skills.FirstOrDefault(s => s.Name == name);
    }

    //remove to education service
    public async Task<Guid> CreateSkillAsync(string name)
    {
      if (string.IsNullOrEmpty(name))
      {
        throw new ArgumentNullException(nameof(name));
      }

      DbSkill dbSkill = _provider.Skills.FirstOrDefault(s => s.Name == name);

      if (dbSkill != null)
      {
        return dbSkill.Id;
      }

      DbSkill skill = new DbSkill
      {
        Id = Guid.NewGuid(),
        Name = name
      };

      _provider.Skills.Add(skill);
      await _provider.SaveAsync();

      return skill.Id;
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

    public async Task<bool> IsCommunicationValueExist(List<string> value)
    {
      if (value == null)
      {
        return false;
      }

      return await _provider.UserCommunications
        .AnyAsync(v => value.Contains(v.Value));
    }

    public async Task<List<DbUser>> SearchAsync(string text)
    {
      return await _provider.Users
        .Where(u => string.Join(" ", u.FirstName, u.MiddleName, u.LastName)
        .Contains(text))
        .ToListAsync();
    }
  }
}
