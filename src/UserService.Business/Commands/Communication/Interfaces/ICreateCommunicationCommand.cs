using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Communication;
using System;

namespace LT.DigitalOffice.UserService.Business.Commands.Communication.Interfaces
{
    [AutoInject]
    public interface ICreateCommunicationCommand
    {
        OperationResultResponse<Guid> Execute(CreateCommunicationRequest request);
    }
}
