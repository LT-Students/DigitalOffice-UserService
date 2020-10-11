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
        /// Returns the user with the specified id from database.
        /// </summary>
        /// <param name="userId">Specified id of user.</param>
        /// <returns>User with specified id.</returns>
        DbUser GetUserInfoById(Guid userId);

        /// <summary>
        /// Adds new user to the database. Returns whether it was successful to add.
        /// </summary>
        /// <param name="user">User to add.</param>
        /// <returns>Guid of added user.</returns>
        Guid CreateUser(DbUser user);

        /// <summary>
        /// Edit existing user. Returns whether it was successful to edit.
        /// </summary>
        /// <param name="user">User to edit.</param>
        /// <returns>Whether it was successful to edit</returns>
        bool EditUser(DbUser user);

        /// <summary>
        /// Returns the user with the specified email from database.
        /// </summary>
        /// <param name="userEmail">Specified user email.</param>
        /// <returns>User model.</returns>
        DbUser GetUserByEmail(string userEmail);
    }
}