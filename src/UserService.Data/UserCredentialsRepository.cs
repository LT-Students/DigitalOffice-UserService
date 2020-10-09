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
    public class UserCredentialsRepository : IUserCredentialsRepository
    {
        private readonly IDataProvider provider;

        public UserCredentialsRepository(IDataProvider provider)
        {
            this.provider = provider;
        }

        public DbUserCredentials GetUserCredentialsByUserId(Guid userId)
        {
            DbUserCredentials userCredentials = provider.UserCredentials.FirstOrDefault(uc => uc.UserId == userId);

            if (userCredentials == null)
            {
                throw new Exception("User credentials with this user id not found.");
            }

            return userCredentials;
        }

        public DbUserCredentials GetUserCredentialsByEmail(string userEmail)
        {
            DbUserCredentials userCredentials = provider.UserCredentials.FirstOrDefault(uc => uc.Email == userEmail);

            if (userCredentials == null)
            {
                throw new Exception("User credentials with this user email not found.");
            }

            return userCredentials;
        }

        public bool EditUserCredentials(DbUserCredentials userCredentials)
        {
            if (!provider.UserCredentials.Any(uc => uc.UserId == userCredentials.UserId))
            {
                throw new Exception("User credentials was not found.");
            }

            provider.UserCredentials.Update(userCredentials);
            provider.Save();

            return true;
        }

        public void CreateUserCredentials(DbUserCredentials userCredentials)
        {
            if (provider.UserCredentials.Any(uc => uc.Email == userCredentials.Email))
            {
                throw new Exception("User credentials is already exist.");
            }

            provider.UserCredentials.Add(userCredentials);
            provider.Save();
        }
    }
}
