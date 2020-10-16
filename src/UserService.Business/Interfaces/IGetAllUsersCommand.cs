using LT.DigitalOffice.UserService.Models.Dto;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Business.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for getting user model by email.
    /// </summary>
    public interface IGetAllUsersCommand
    {
        /// <summary>
        /// Returns the list of all user models.
        /// </summary>
        /// <returns>List of all user models.</returns>
        IEnumerable<User> Execute();
    }
}
