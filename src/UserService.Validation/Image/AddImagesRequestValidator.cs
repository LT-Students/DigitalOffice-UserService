using FluentValidation;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Images;
using LT.DigitalOffice.UserService.Validation.Image.Interfaces;

namespace LT.DigitalOffice.UserService.Validation.Avatars
{
  public class AddImagesRequestValidator : AbstractValidator<CreateImageRequest>, IAddImagesRequestValidator
  {
    public AddImagesRequestValidator(
      IImageContentValidator imageContentValidator,
      IImageExtensionValidator imageExtensionValidator,
      IUserRepository userRepository,
      ICertificateRepository certificateRepository,
      IEducationRepository educationRepository)
    {
      RuleFor(x => x)
        .MustAsync(async (x, _) =>
          await userRepository.GetAsync(x.EntityId) != null
          || certificateRepository.Get(x.EntityId) != null
          || educationRepository.Get(x.EntityId) != null)
        .WithMessage("Entity doesn't exist.")
        .Must(x => !(x.EntityType != EntityType.User && x.IsCurrentAvatar))
        .WithMessage("Only users can have avatars.");

      RuleFor(x => x.Content)
        .SetValidator(imageContentValidator);

      RuleFor(x => x.Extension)
        .SetValidator(imageExtensionValidator);
    }
  }
}
