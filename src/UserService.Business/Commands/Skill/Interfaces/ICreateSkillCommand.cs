using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Skill.Interfaces
{
  [AutoInject]
  public interface ICreateSkillCommand
  {
    Task<OperationResultResponse<Guid>> ExecuteAsync(CreateSkillRequest request);
  }
}
