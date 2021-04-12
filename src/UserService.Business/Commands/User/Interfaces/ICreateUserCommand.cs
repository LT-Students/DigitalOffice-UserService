using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Responses;
using System;

namespace LT.DigitalOffice.UserService.Business.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for adding a new user.
    /// </summary>
    [AutoInject]
    public interface ICreateUserCommand
    {
        /// <summary>
        /// Adds a new user. Returns true if it succeeded to add a user, otherwise false.
        /// </summary>
        OperationResultResponse<Guid> Execute(CreateUserRequest request);
    }
}