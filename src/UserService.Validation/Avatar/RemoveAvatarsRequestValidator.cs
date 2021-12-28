using FluentValidation;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Avatar;
using LT.DigitalOffice.UserService.Validation.Image.Interfaces;

namespace LT.DigitalOffice.UserService.Validation.Avatar
{
  public class RemoveAvatarsRequestValidator : AbstractValidator<RemoveAvatarsRequest>, IRemoveAvatarsRequestValidator
  {
    public RemoveAvatarsRequestValidator()
    {
      RuleFor(x => x.AvatarsIds)
        .NotEmpty().WithMessage("Images Ids can not be null.");
    }
  }
}
