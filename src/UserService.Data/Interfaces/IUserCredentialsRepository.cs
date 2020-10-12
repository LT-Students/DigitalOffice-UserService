﻿using LT.DigitalOffice.UserService.Models.Db;
using System;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
    /// <summary>
    /// Represents interface of repository in repository pattern.
    /// Provides methods for working with the database of UserService.
    /// </summary>
    public interface IUserCredentialsRepository
    {
        /// <summary>
        /// Returns the user credentials with the specified user id from database.
        /// </summary>
        /// <param name="userId">Specified Id of user.</param>
        /// <returns>User credentials model.</returns>
        DbUserCredentials GetUserCredentialsByUserId(Guid userId);

        /// <summary>
        /// Returns the user credentials with the specified user email from database.
        /// </summary>
        /// <param name="login">Specified login of user.</param>
        /// <returns>User credentials model.</returns>
        DbUserCredentials GetUserCredentialsByLogin(string login);

        /// <summary>
        /// Edit existing user credentials. Returns whether it was successful to edit.
        /// </summary>
        /// <param name="userCredentials">User credentials to edit.</param>
        /// <returns>Whether it was successful to edit.</returns>
        bool EditUserCredentials(DbUserCredentials userCredentials);
    }
}
