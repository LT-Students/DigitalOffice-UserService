using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using System;
using System.Linq;

namespace LT.DigitalOffice.UserService.Data
{
    /// <summary>
    /// Represents interface of repository. Provides method for getting user model from database.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly IDataProvider provider;

        public UserRepository(IDataProvider provider)
        {
            this.provider = provider;
        }

        public Guid UserCreate(DbUser user)
        {
            if (provider.Users.Any(users => user.Email == users.Email))
            {
                throw new Exception("Email is already taken.");
            }

            provider.Users.Add(user);
            provider.Save();

            return user.Id;
        }

        public DbUser GetUserInfoById(Guid userId)
            => provider.Users.FirstOrDefault(dbUser => dbUser.Id == userId) ??
               throw new Exception("User with this id not found.");

        public bool EditUser(DbUser user)
        {
            if (!provider.Users.Any(users => user.Id == users.Id))
            {
                throw new Exception("User was not found.");
            }

            provider.Users.Update(user);
            provider.Save();

            return true;
        }

        public DbUser GetUserByEmail(string userEmail)
        {
            DbUser user = provider.Users.FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            return user;
        }
    }
}
