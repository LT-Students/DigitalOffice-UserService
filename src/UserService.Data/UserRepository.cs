using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Data
{
    /// <inheritdoc/>
    public class UserRepository : IUserRepository
    {
        private readonly IDataProvider _provider;

        public UserRepository(IDataProvider provider)
        {
            _provider = provider;
        }

        public Guid CreateUser(DbUser dbUser, DbUserCredentials dbUserCredentials)
        {
            if (_provider.Users.Any(u => u.Email == dbUser.Email))
            {
                throw new BadRequestException("Email is already taken.");
            }

            if (_provider.UserCredentials.Any(uc => uc.Login == dbUserCredentials.Login))
            {
                throw new BadRequestException("User credentials is already exist.");
            }

            _provider.Users.Add(dbUser);
            _provider.UserCredentials.Add(dbUserCredentials);
            _provider.Save();

            return dbUser.Id;
        }

        public DbUser GetUserInfoById(Guid userId)
            => _provider.Users.FirstOrDefault(dbUser => dbUser.Id == userId) ??
               throw new NotFoundException($"User with this id: '{userId}' was not found.");

        public bool EditUser(DbUser user)
        {
            if (!_provider.Users.Any(users => user.Id == users.Id))
            {
                throw new NotFoundException($"User with this id: '{user.Id}' was not found.");
            }

            _provider.Users.Update(user);
            _provider.Save();

            return true;
        }

        public DbUser GetUserByEmail(string userEmail)
        {
            DbUser dbUser = _provider.Users.FirstOrDefault(uc => uc.Email == userEmail);

            if (dbUser == null)
            {
                throw new NotFoundException($"User with this email: '{userEmail}' was not found.");
            }

            return dbUser;
        }

        public IEnumerable<DbUser> GetUsersByIds(IEnumerable<Guid> usersIds)
        {
            if (!usersIds.Any())
            {
                throw new BadRequestException();
            }

            var dbUsers = _provider.Users.Where(user => usersIds.Contains(user.Id)).ToList();

            if (!dbUsers.Any())
            {
                throw new NotFoundException("Users were not found.");
            }

            return dbUsers;
        }

        public IEnumerable<DbUser> GetAllUsers(int skipCount, int takeCount, string userNameFilter)
        {
            if (userNameFilter != null)
            {
               return _provider.Users
                    .Select(user => new { user.Id, FIO = $"{user.LastName} {user.FirstName} {user.MiddleName}", Info = user })
                    .AsEnumerable()
                    .Where(user => user.FIO.Contains(userNameFilter))
                    .Select(user => user.Info)
                    .Skip(skipCount)
                    .Take(takeCount)
                    .AsEnumerable();
            }

            return _provider.Users
                .Skip(skipCount)
                .Take(takeCount)
                .AsEnumerable();
        }

        public DbSkill FindSkillByName(string name)
        {
            return _provider.Skills.FirstOrDefault(s => s.SkillName == name);
        }

        public Guid CreateSkill(string name)
        {
            var dbSkill = _provider.Skills.FirstOrDefault(s => s.SkillName == name);

            if (dbSkill != null)
            {
                throw new BadRequestException();
            }

            var skill = new DbSkill { Id = Guid.NewGuid(), SkillName = name };

            _provider.Skills.Add(skill);
            _provider.Save();

            return skill.Id;
        }
    }
}
