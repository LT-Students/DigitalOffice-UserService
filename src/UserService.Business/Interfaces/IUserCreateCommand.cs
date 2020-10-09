using System;
using LT.DigitalOffice.UserService.Models.Dto;

namespace LT.DigitalOffice.UserService.Business.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for adding a new user.
    /// </summary>
    public interface IUserCreateCommand
    {
        /// <summary>
        ///  Adds a new user. Returns true if it succeeded to add a user, otherwise false.
        /// </summary>
        /// <param name="request">User data.</param>
        /// <returns>Guid of added user.</returns>
        Guid Execute(UserCreateRequest request);
    }
}