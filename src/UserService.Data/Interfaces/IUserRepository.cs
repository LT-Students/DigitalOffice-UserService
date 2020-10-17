﻿using LT.DigitalOffice.UserService.Models.Db;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
    /// <summary>
    /// Represents interface of repository in repository pattern.
    /// Provides methods for working with the database of UserService.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Returns the dbUser with the specified id from database.
        /// </summary>
        /// <param name="userId">Specified id of dbUser.</param>
        /// <returns>User with specified id.</returns>
        DbUser GetUserInfoById(Guid userId);

        /// <summary>
        /// Adds new dbUser to the database. Returns whether it was successful to add.
        /// </summary>
        /// <param name="dbUser">User to add.</param>
        /// <param name="dbUserCredentials">User credentials to add.</param>
        /// <returns>ID of added dbUser.</returns>
        Guid CreateUser(DbUser dbUser, DbUserCredentials dbUserCredentials);

        /// <summary>
        /// Edit existing dbUser. Returns whether it was successful to edit.
        /// </summary>
        /// <param name="user">User to edit.</param>
        /// <returns>Whether it was successful to edit.</returns>
        bool EditUser(DbUser user);

        /// <summary>
        /// Returns the dbUser with the specified email from database.
        /// </summary>
        /// <param name="userEmail">Specified dbUser email.</param>
        /// <returns>User model.</returns>
        DbUser GetUserByEmail(string userEmail);

        /// <summary>
        /// Returns the list of user models using pagination and filter by full name.
        /// </summary>
        /// <param name="skipCount">Number of pages to skip.</param>
        /// <param name="takeCount">Number of users on one page.</param>
        /// <param name="nameFilter">Full name that is wanted to be found.</param>
        /// <returns>List of user models.</returns>
        IEnumerable<DbUser> GetAllUsers(int skipCount, int takeCount, string nameFilter);
    }
}