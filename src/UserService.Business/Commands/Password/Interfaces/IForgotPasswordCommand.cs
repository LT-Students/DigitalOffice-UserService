using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;

namespace LT.DigitalOffice.UserService.Business.Commands.Password.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// </summary>
    [AutoInject]
    public interface IForgotPasswordCommand
    {
        /// <summary>
        /// Method for getting user description by email and sent request in Message Service.
        /// </summary>
        OperationResultResponse<bool> Execute(string userEmail);
    }
}
