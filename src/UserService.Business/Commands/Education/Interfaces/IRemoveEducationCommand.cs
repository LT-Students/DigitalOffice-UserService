using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using System;

namespace LT.DigitalOffice.UserService.Business.Commands.Education.Interfaces
{
    [AutoInject]
    public interface IRemoveEducationCommand
    {
        OperationResultResponse<bool> Execute(Guid educationId);
    }
}
