using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Avatar;

namespace LT.DigitalOffice.UserService.Validation.Image.Interfaces
{
  [AutoInject]
  public interface ICreateAvatarRequestValidator : IValidator<CreateAvatarRequest>
  {
  }
}
