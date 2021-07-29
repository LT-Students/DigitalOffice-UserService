using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials.Filters;
using System;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
    /// <summary>
    /// Represents interface of repository in repository pattern.
    /// Provides methods for working with the database of UserService.
    /// </summary>
    [AutoInject]
    public interface IUserCredentialsRepository
    {
        /// <summary>
        /// Returns the user credentials.
        /// </summary>
        DbUserCredentials Get(GetCredentialsFilter filter);

        Guid Create(DbUserCredentials dbUserCredentials);

        bool Remove(Guid userId);

        /// <summary>
        /// Edit existing user credentials. Returns whether it was successful to edit.
        /// </summary>
        /// <param name="userCredentials">User credentials to edit.</param>
        /// <returns>Whether it was successful to edit.</returns>
        bool Edit(DbUserCredentials userCredentials);

        bool IsLoginExist(string login);

        bool IsCredentialsExist(Guid userId);
    }
}
