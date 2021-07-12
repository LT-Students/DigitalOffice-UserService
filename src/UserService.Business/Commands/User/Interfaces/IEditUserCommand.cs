using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using Microsoft.AspNetCore.JsonPatch;
using System;

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
        OperationResultResponse<bool> Execute(Guid userId, JsonPatchDocument<EditUserRequest> patch);
    }
}
