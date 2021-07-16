using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Responses.User;
using System;

namespace LT.DigitalOffice.UserService.Business.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for getting list of user models with pagination and filter by full name.
    /// </summary>
    [AutoInject]
    public interface IFindUserCommand
    {
        /// <summary>
        /// Returns the list of user models using pagination and filter by full name.
        /// </summary>
        UsersResponse Execute(int skipCount, int takeCount, Guid? departmentId);
    }
}
