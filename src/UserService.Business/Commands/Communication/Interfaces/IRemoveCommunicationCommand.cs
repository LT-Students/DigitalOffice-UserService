using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using System;

namespace LT.DigitalOffice.UserService.Business.Commands.Communication.Interfaces
{
    [AutoInject]
    public interface IRemoveCommunicationCommand
    {
        OperationResultResponse<bool> Execute(Guid communicationId);
    }
}
