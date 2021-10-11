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
        dbUsers = dbUsers.Where(u => u.FirstName.Contains(filter.Name) || u.LastName.Contains(filter.Name));
      }

      if (filter.IncludeCommunications)
      {
        dbUsers = dbUsers.Include(u => u.Communications);

        if (!string.IsNullOrEmpty(filter.Email?.Trim()))
        {
          dbUsers = dbUsers.Where(u => u.Communications
              .Any(c => c.Type == (int)CommunicationType.Email &&
                        c.Value == filter.Email));
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

    public async Task<Guid> Create(DbUser dbUser)
    {
      if (dbUser == null)
      {
        throw new ArgumentNullException(nameof(dbUser));
      }

      _provider.Users.Add(dbUser);
      await _provider.SaveAsync();

      return dbUser.Id;
    }

    public async Task CreatePending(DbPendingUser dbPendingUser)
    {
      if (dbPendingUser == null)
      {
        throw new ArgumentNullException(nameof(dbPendingUser));
      }

      if (_provider.PendingUsers.FirstOrDefault(p => p.UserId == dbPendingUser.UserId) != null)
      {
        throw new BadRequestException($"Pending user with Id:{dbPendingUser.UserId} already exists");
      }

      _provider.PendingUsers.Add(dbPendingUser);
      await _provider.SaveAsync();
    }

    public DbUser Get(Guid id)
    {
      GetUserFilter filter = new()
      {
        UserId = id
      };

      return Get(filter);
    }

    public DbUser Get(GetUserFilter filter)
    {
      if (filter == null)
      {
        throw new ArgumentNullException(nameof(filter));
      }

      IQueryable<DbUser> dbUsers = _provider.Users
          .AsSingleQuery()
          .AsQueryable();

      return CreateGetPredicates(filter, dbUsers).FirstOrDefault();
    }

    public List<DbUser> Get(IEnumerable<Guid> userIds)
    {
      if (userIds == null)
      {
        return null;
      }

      return _provider.Users.Where(x => userIds.Contains(x.Id)).ToList();
    }

    public List<Guid> AreExistingIds(List<Guid> userIds)
    {
      return _provider.Users
          .Where(u => userIds.Contains(u.Id) && u.IsActive)
          .Select(u => u.Id).ToList();
    }

    public async Task<bool> EditUser(Guid userId, JsonPatchDocument<DbUser> userPatch)
    {
      if (userPatch == null)
      {
        throw new ArgumentNullException(nameof(userPatch));
      }

      DbUser dbUser = _provider.Users.FirstOrDefault(x => x.Id == userId) ??
                      throw new NotFoundException($"User with ID '{userId}' was not found.");

      userPatch.ApplyTo(dbUser);
      dbUser.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
      dbUser.ModifiedAtUtc = DateTime.UtcNow;
      await _provider.SaveAsync();

      return true;
    }

    public DbSkill FindSkillByName(string name)
    {
      return _provider.Skills.FirstOrDefault(s => s.Name == name);
    }

    public async Task<Guid> CreateSkill(string name)
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
    public async Task<bool> SwitchActiveStatus(Guid userId, bool status)
    {
      DbUser dbUser = _provider.Users.FirstOrDefault(u => u.Id == userId);
      if (dbUser == null)
      {
        throw new NotFoundException($"User with ID '{userId}' was not found.");
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

    public (List<DbUser> dbUsers, int totalCount) Find(FindUsersFilter filter)
    {
      if (filter.SkipCount < 0)
      {
        throw new BadRequestException("Skip count can't be less than 0.");
      }

      if (filter.TakeCount < 1)
      {
        throw new BadRequestException("Take count can't be less than 1.");
      }

      return (
        filter.IncludeDeactivated ?
          _provider.Users.Skip(filter.SkipCount).Take(filter.TakeCount).ToList() :
          _provider.Users.Where(x => x.IsActive).Skip(filter.SkipCount).Take(filter.TakeCount).ToList(),

        _provider.Users.Count());
    }

    public DbPendingUser GetPendingUser(Guid userId)
    {
      return _provider.PendingUsers.FirstOrDefault(pu => pu.UserId == userId);
    }

    public async Task DeletePendingUser(Guid userId)
    {
      DbPendingUser dbPendingUser = _provider.PendingUsers.FirstOrDefault(pu => pu.UserId == userId);

      _provider.PendingUsers.Remove(dbPendingUser);
      await _provider.SaveAsync();
    }

    public bool IsUserExist(Guid userId)
    {
      return _provider.Users.FirstOrDefault(u => u.Id == userId) != null;
    }

    public bool IsCommunicationValueExist(List<string> value)
    {
      return _provider.UserCommunications.Any(v => value.Contains(v.Value));
    }

    public List<DbUser> Search(string text)
    {
      return _provider.Users.Where(u => string.Join(" ", u.FirstName, u.MiddleName, u.LastName).Contains(text)).ToList();
    }

    public async Task<bool> RemoveAvatar(Guid userId)
    {
      DbUser dbUser = Get(userId);

      if (dbUser == null)
      {
        return false;
      }

      dbUser.AvatarFileId = null;

      await _provider.SaveAsync();

      return true;
    }
  }
}
