using UserService.Models.Dto;

namespace UserService.Business.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for getting user model by email.
    /// </summary>
    public interface IGetUserByEmailCommand
    {
        /// <summary>
        /// Returns the user model with the specified email.
        /// </summary>
        /// <param name="userEmail">Specified email of user.</param>
        /// <returns>User model with specified email.</returns>
        User Execute(string userEmail);
    }
}
