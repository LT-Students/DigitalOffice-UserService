using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Interfaces
{
    /// <summary>
    /// Represents interface for a command in command pattern.
    /// Provides method for editing an existing user.
    /// </summary>
    [AutoInject]
    public interface IEditUserCommand
    {
        /// <summary>
        /// Editing an existing user. Returns true if it succeeded to edit a user, otherwise false.
        /// </summary>
        Task<OperationResultResponse<bool>> Execute(Guid userId, JsonPatchDocument<EditUserRequest> patch);
    }
}
