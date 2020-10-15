using LT.DigitalOffice.UserService.Models.Dto;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Business.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for getting users model by ids.
    /// </summary>
    public interface IGetUsersByIdsCommand
    {
        /// <summary>
        /// Returns list of users models with specified ids.
        /// </summary>
        /// <param name="usersIds">Specified ids of users.</param>
        /// <returns>List of users models with specified ids.</returns>
        List<User> Execute(IEnumerable<Guid> usersIds);
    }
}
