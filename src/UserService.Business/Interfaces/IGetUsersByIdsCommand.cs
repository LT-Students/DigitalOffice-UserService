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
        /// Returns collection of users models with specified ids.
        /// </summary>
        /// <param name="usersIds">Specified ids of users.</param>
        /// <returns>Collection of users models with specified ids.</returns>
        IEnumerable<User> Execute(IEnumerable<Guid> usersIds);
    }
}
