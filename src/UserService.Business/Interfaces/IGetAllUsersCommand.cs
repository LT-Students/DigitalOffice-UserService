using LT.DigitalOffice.UserService.Models.Dto;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Business.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for getting list of user models with pagination and filter by full name.
    /// </summary>
    public interface IGetAllUsersCommand
    {
        /// <summary>
        /// Returns the list of user models using pagination and filter by full name.
        /// </summary>
        /// <param name="skipCount">Number of pages to skip.</param>
        /// <param name="takeCount">Number of users on one page.</param>
        /// <param name="userNameFilter">User full name or its part that is wanted to be found.</param>
        /// <returns>List of user models.</returns>
        IEnumerable<User> Execute(int skipCount, int takeCount, string userNameFilter);
    }
}
