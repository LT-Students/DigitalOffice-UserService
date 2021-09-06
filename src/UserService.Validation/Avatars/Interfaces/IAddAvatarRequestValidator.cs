using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Avatars;

namespace LT.DigitalOffice.UserService.Validation.Avatars.Interfaces
{
  [AutoInject]
  public interface IAddAvatarRequestValidator : IValidator<AddAvatarRequest>
  {
  }
}
