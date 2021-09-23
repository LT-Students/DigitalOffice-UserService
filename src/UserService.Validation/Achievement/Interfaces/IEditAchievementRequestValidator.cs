using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Achievement;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.UserService.Validation.Achievement.Interfaces
{
  [AutoInject]
  public interface IEditAchievementRequestValidator : IValidator<JsonPatchDocument<EditAchievementRequest>>
  {
  }
}
