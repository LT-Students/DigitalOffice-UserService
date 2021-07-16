using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using System;

namespace LT.DigitalOffice.UserService.Business.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for getting result of operation to disabling user.
    /// </summary>
    [AutoInject]
    public interface IDisableUserCommand
    {
        /// <summary>
        /// The result of operation with boolean value.
        /// </summary>
        OperationResultResponse<bool> Execute(Guid userId);
    }
}
