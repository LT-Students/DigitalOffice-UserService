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
               throw new Exception("User with this id not found.");

        public bool EditUser(DbUser user)
        {
            if (!_provider.Users.Any(users => user.Id == users.Id))
            {
                throw new Exception("User was not found.");
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
                throw new Exception("User credentials not found.");
            }

            return dbUser;
        }

        public IEnumerable<DbUser> GetAllUsers()
        {
            var dbUsers =  _provider.Users.AsEnumerable();

            if (dbUsers == null)
            {
                throw new NotFoundException("Users were not found.");
            }

            return dbUsers;
        }
    }
}
