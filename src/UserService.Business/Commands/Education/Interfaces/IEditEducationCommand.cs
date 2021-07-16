using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using Microsoft.AspNetCore.JsonPatch;
using System;

namespace LT.DigitalOffice.UserService.Business.Commands.Education.Interfaces
{
    [AutoInject]
    public interface IEditEducationCommand
    {
        OperationResultResponse<bool> Execute(Guid educationId, JsonPatchDocument<EditEducationRequest> request);
    }
}
