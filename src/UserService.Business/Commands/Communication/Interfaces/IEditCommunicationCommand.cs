using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Communication;
using Microsoft.AspNetCore.JsonPatch;
using System;

namespace LT.DigitalOffice.UserService.Business.Commands.Communication.Interfaces
{
    [AutoInject]
    public interface IEditCommunicationCommand
    {
        OperationResultResponse<bool> Execute(Guid communicationId, JsonPatchDocument<EditCommunicationRequest> request);
    }
}
