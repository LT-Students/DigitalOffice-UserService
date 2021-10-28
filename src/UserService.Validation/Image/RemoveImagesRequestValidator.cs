using FluentValidation;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Images;
using LT.DigitalOffice.UserService.Validation.Image.Interfaces;
using LT.DigitalOffice.UserService.Validation.Helpers.Interfaces;

namespace LT.DigitalOffice.UserService.Validation.Image
{
  public class RemoveImagesRequestValidator : AbstractValidator<RemoveImagesRequest>, IRemoveImagesRequestValidator
  {
    public RemoveImagesRequestValidator(
      ICheckImagesToUserAffiliationHelper helper)
    {
      RuleFor(x => x.ImagesIds)
        .NotEmpty().WithMessage("Images Ids can not be null.");

      RuleFor(request => request)
        .MustAsync(async (request, _) => await helper.CheckAffiliationAsync(request.ImagesIds, request.EntityId))
        .WithMessage("Images Ids must belong to only one entity.");
    }
  }
}
