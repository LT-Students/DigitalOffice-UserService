using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Images;

namespace LT.DigitalOffice.UserService.Validation.Image.Interfaces
{
  [AutoInject]
  public interface IUpdateAvatarRequestValidator : IValidator<UpdateAvatarRequest>
  {
  }
}
