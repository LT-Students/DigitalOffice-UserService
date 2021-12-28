using FluentValidation;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Avatar;
using LT.DigitalOffice.UserService.Validation.Image.Interfaces;

namespace LT.DigitalOffice.UserService.Validation.Avatars
{
  public class CreateAvatarRequestValidator : AbstractValidator<CreateAvatarRequest>, ICreateAvatarRequestValidator
  {
    public CreateAvatarRequestValidator(
      IImageContentValidator imageContentValidator,
      IImageExtensionValidator imageExtensionValidator)
    {
      RuleFor(x => x.Content)
        .SetValidator(imageContentValidator);

      RuleFor(x => x.Extension)
        .SetValidator(imageExtensionValidator);
    }
  }
}
