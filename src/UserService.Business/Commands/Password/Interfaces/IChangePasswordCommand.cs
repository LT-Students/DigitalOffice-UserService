using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Password.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for changing user password.
    /// </summary>
    [AutoInject]
    public interface IChangePasswordCommand
    {
        /// <summary>
        /// Change password of the user with specified login.
        /// </summary>
        Task<OperationResultResponse<bool>> Execute(ChangePasswordRequest request);
    }
}
