using FluentValidation;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Images;
using LT.DigitalOffice.UserService.Validation.Image.Interfaces;

namespace LT.DigitalOffice.UserService.Validation.Avatars
{
  public class AddImagesRequestValidator : AbstractValidator<AddImagesRequest>, IAddImagesRequestValidator
  {
    public AddImagesRequestValidator(
      IAddImageRequestValidator imageValidator,
      IUserRepository userRepository)
    {
      RuleFor(x => x.EntityId)
        .Must(x => userRepository.Get(x) != null).WithMessage("Entity doesn't exist.");

      RuleForEach(x => x.Images)
        .SetValidator(imageValidator);
    }
  }
}
