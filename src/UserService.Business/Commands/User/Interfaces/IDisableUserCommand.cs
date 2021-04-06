using LT.DigitalOffice.UserService.Models.Dto.Responses;
using System;

namespace LT.DigitalOffice.UserService.Business.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for getting result of operation to disabling user.
    /// </summary>
    public interface IDisableUserCommand
    {
        /// <summary>
        /// The result of operation with boolean value.
        /// </summary>
        OperationResultResponse<bool> Execute(Guid userId);
    }
}
