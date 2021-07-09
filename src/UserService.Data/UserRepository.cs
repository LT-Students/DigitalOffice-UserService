using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters;
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

            if (filter.IncludeCommunications.HasValue && filter.IncludeCommunications.Value)
            {
                dbUsers = dbUsers.Include(u => u.Communications);

                if (!string.IsNullOrEmpty(filter.Email?.Trim()))
                {
                    dbUsers = dbUsers.Where(u => u.Communications
                        .Any(c => c.Type == (int)CommunicationType.Email &&
                                  c.Value == filter.Email));
                }
            }

            if (filter.IncludeCertificates.HasValue && filter.IncludeCertificates.Value)
            {
                dbUsers = dbUsers.Include(u => u.Certificates);
            }

            if (filter.IncludeEducations.HasValue && filter.IncludeEducations.Value)
            {
                dbUsers = dbUsers.Include(u => u.Educations.Where(e => e.IsActive));
            }

            if (filter.IncludeAchievements.HasValue && filter.IncludeAchievements.Value)
            {
                dbUsers = dbUsers.Include(u => u.Achievements).ThenInclude(a => a.Achievement);
            }

            if (filter.IncludeSkills.HasValue && filter.IncludeSkills.Value)
            {
                dbUsers = dbUsers.Include(u => u.Skills).ThenInclude(s => s.Skill);
            }

            return dbUsers;
        }

        public UserRepository(IDataProvider provider)
        {
            _provider = provider;
        }

        public Guid Create(DbUser dbUser, string password)
        {
            DbPendingUser dbPendingUser = new()
            {
                UserId = dbUser.Id,
                Password = password
            };

            _provider.Users.Add(dbUser);
            _provider.PendingUsers.Add(dbPendingUser);
            _provider.Save();

            return dbUser.Id;
        }

        public Guid Create(DbUser dbUser, DbUserCredentials credentials)
        {
            if (dbUser == null)
            {
                throw new ArgumentNullException(nameof(dbUser));
            }

            if (credentials == null)
            {
                throw new ArgumentNullException(nameof(credentials));
            }

            _provider.Users.Add(dbUser);
            _provider.UserCredentials.Add(credentials);
            _provider.Save();

            return dbUser.Id;
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

        public bool EditUser(Guid userId, JsonPatchDocument<DbUser> userPatch)
        {
            if (userPatch == null)
            {
                throw new ArgumentNullException(nameof(userPatch));
            }

            DbUser dbUser = _provider.Users.FirstOrDefault(x => x.Id == userId) ??
                            throw new NotFoundException($"User with ID '{userId}' was not found.");

            userPatch.ApplyTo(dbUser);
            _provider.Save();

            return true;
        }

        public DbSkill FindSkillByName(string name)
        {
            return _provider.Skills.FirstOrDefault(s => s.SkillName == name);
        }

        public Guid CreateSkill(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var dbSkill = _provider.Skills.FirstOrDefault(s => s.SkillName == name);

            if (dbSkill != null)
            {
                return dbSkill.Id;
            }

            var skill = new DbSkill
            {
                Id = Guid.NewGuid(),
                SkillName = name
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
            _provider.Save();

            return true;
        }

        public List<DbUser> Find(int skipCount, int takeCount, out int totalCount)
        {
            if (takeCount <= 0)
            {
                throw new BadRequestException("Take count can't be equal or less than 0.");
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

        public bool IsExistUser(Guid userId)
        {
            return _provider.Users.FirstOrDefault(u => u.Id == userId) != null;
        }

        public bool IsExistCommunicationValue(List<string> value)
        {
            return _provider.UserCommunications.Any(v => value.Contains(v.Value));
        }

        public List<DbUser> Search(string text)
        {
            return _provider.Users
                                .ToList()
                                .Where(u => string.Join(" ", u.FirstName, u.MiddleName, u.LastName).Contains(text, StringComparison.OrdinalIgnoreCase))
                                .ToList();
        }
    }
}
