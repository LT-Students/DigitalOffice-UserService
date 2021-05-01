using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using LT.DigitalOffice.UserService.Models.Dto.Responses;
using System;

namespace LT.DigitalOffice.UserService.Business.Commands.User.Interfaces.Education
{
    [AutoInject]
    public interface ICreateEducationCommand
    {
        OperationResultResponse<Guid> Execute(CreateEducationRequest request);
    }
}
