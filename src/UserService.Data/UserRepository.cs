using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

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

    public Guid Create(DbUser dbUser)
    {
      if (dbUser == null)
      {
        throw new ArgumentNullException(nameof(dbUser));
      }

      _provider.Users.Add(dbUser);
      _provider.Save();

      return dbUser.Id;
    }

    public void CreatePending(DbPendingUser dbPendingUser)
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
      _provider.Save();
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

      var dbUsers = _provider.Users
          .AsSingleQuery()
          .AsQueryable();

      return CreateGetPredicates(filter, dbUsers).FirstOrDefault();
    }

    public List<DbUser> Get(IEnumerable<Guid> userIds)
    {
      return _provider.Users.Where(x => userIds.Contains(x.Id)).ToList();
    }

    public List<Guid> AreExistingIds(List<Guid> userIds)
    {
      return _provider.Users
          .Where(u => userIds.Contains(u.Id) && u.IsActive)
          .Select(u => u.Id).ToList();
    }

    public bool EditUser(Guid userId, JsonPatchDocument<DbUser> userPatch)
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
      _provider.Save();

      return true;
    }

    public DbSkill FindSkillByName(string name)
    {
      return _provider.Skills.FirstOrDefault(s => s.Name == name);
    }

    public Guid CreateSkill(string name)
    {
      if (string.IsNullOrEmpty(name))
      {
        throw new ArgumentNullException(nameof(name));
      }

      var dbSkill = _provider.Skills.FirstOrDefault(s => s.Name == name);

      if (dbSkill != null)
      {
        return dbSkill.Id;
      }

      var skill = new DbSkill
      {
        Id = Guid.NewGuid(),
        Name = name
      };

      _provider.Skills.Add(skill);
      _provider.Save();

      return skill.Id;
    }

    /// <inheritdoc />
    public bool SwitchActiveStatus(Guid userId, bool status)
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
      _provider.Save();

      return true;
    }

    public List<DbUser> Find(int skipCount, int takeCount, out int totalCount)
    {
      if (skipCount < 0)
      {
        throw new BadRequestException("Skip count can't be less than 0.");
      }

      if (takeCount < 1)
      {
        throw new BadRequestException("Take count can't be less than 1.");
      }

      totalCount = _provider.Users.Count();

      return _provider.Users.Skip(skipCount).Take(takeCount).ToList();
    }

    public DbPendingUser GetPendingUser(Guid userId)
    {
      return _provider.PendingUsers.FirstOrDefault(pu => pu.UserId == userId);
    }

    public void DeletePendingUser(Guid userId)
    {
      DbPendingUser dbPendingUser = _provider.PendingUsers.FirstOrDefault(pu => pu.UserId == userId);

      _provider.PendingUsers.Remove(dbPendingUser);
      _provider.Save();
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

    public bool UpdateAvatar(Guid userId, Guid? avatarId)
    {
      DbUser dbUser = Get(userId);

      if (dbUser == null)
      {
        return false;
      }

      dbUser.AvatarFileId = avatarId;

      _provider.Save();

      return true;
    }
  }
}
