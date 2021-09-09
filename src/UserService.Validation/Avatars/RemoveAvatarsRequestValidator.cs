using FluentValidation;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Avatars;
using LT.DigitalOffice.UserService.Validation.Avatars.Interfaces;
using LT.DigitalOffice.UserService.Validation.Helpers.Interfaces;

namespace LT.DigitalOffice.UserService.Validation.Avatars
{
  public class RemoveAvatarsRequestValidator : AbstractValidator<RemoveAvatarsRequest>, IRemoveAvatarsRequestValidator
  {
    public RemoveAvatarsRequestValidator(
      ICheckImagesToUserAffiliationHelper helper)
    {
      RuleFor(x => x.AvatarIds)
        .NotEmpty().WithMessage("Images Ids can not be null.");

      RuleFor(request => request)
        .Must(request => helper.CheckAffiliation(request.AvatarIds, request.UserId))
        .WithMessage("Images Ids must belong to only one user!");
    }
  }
}
