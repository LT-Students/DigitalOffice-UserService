using LT.DigitalOffice.UserService.Models.Db;
using System;

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
    }
}