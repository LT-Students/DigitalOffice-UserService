using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Achievement;

namespace LT.DigitalOffice.UserService.Validation.Achievement.Interfaces
{
  [AutoInject]
  public interface ICreateAchievementRequestValidator : IValidator<CreateAchievementRequest>
  {
  }
}
