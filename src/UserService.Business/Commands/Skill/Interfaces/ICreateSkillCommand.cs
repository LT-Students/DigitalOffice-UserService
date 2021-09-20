using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto;
using System;

namespace LT.DigitalOffice.UserService.Business.Commands.Skill.Interfaces
{
    [AutoInject]
    public interface ICreateSkillCommand
    {
        OperationResultResponse<Guid> Execute(CreateSkillRequest request);
    }
}
