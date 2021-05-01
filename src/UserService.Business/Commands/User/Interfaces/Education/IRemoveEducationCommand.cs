using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Responses;
using System;

namespace LT.DigitalOffice.UserService.Business.Commands.User.Interfaces.Education
{
    [AutoInject]
    public interface IRemoveEducationCommand
    {
        OperationResultResponse<bool> Execute(Guid userId, Guid educationId);
    }
}
