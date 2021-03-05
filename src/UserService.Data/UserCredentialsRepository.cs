using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using System;
using System.Linq;

namespace LT.DigitalOffice.UserService.Data
{
    /// <inheritdoc />
    public class UserCredentialsRepository : IUserCredentialsRepository
    {
        private readonly IDataProvider _provider;

        public UserCredentialsRepository(IDataProvider provider)
        {
            _provider = provider;
        }

        public DbUserCredentials GetUserCredentialsByUserId(Guid userId)
        {
            DbUserCredentials userCredentials = _provider.UserCredentials.FirstOrDefault(uc => uc.UserId == userId);

            if (userCredentials == null)
            {
                throw new NotFoundException($"User credentials with this user id: '{userId}' not found.");
            }

            return userCredentials;
        }

        public DbUserCredentials GetUserCredentialsByLogin(string login)
        {
            DbUserCredentials userCredentials = _provider.UserCredentials.FirstOrDefault(uc => uc.Login == login);

            if (userCredentials == null)
            {
                throw new NotFoundException($"User credentials with this user login: '{login}' not found.");
            }

            return userCredentials;
        }

        public bool EditUserCredentials(DbUserCredentials userCredentials)
        {
            if (!_provider.UserCredentials.Any(uc => uc.UserId == userCredentials.UserId))
            {
                throw new NotFoundException("User credentials was not found.");
            }

            _provider.UserCredentials.Update(userCredentials);
            _provider.Save();

            return true;
        }
    }
}
