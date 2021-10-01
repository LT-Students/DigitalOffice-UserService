using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto;

namespace LT.DigitalOffice.UserService.Validation.Skill.Interfaces
{
  [AutoInject]
  public interface ICreateSkillRequestValidator : IValidator<CreateSkillRequest>
  {
  }
}
