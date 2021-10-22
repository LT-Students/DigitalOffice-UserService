using FluentValidation;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Images;
using LT.DigitalOffice.UserService.Validation.Image.Interfaces;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Validation.Image
{
  public class UpdateAvatarRequestValidator : AbstractValidator<UpdateAvatarRequest>, IUpdateAvatarRequestValidator
  {
    public UpdateAvatarRequestValidator(
      IImageContentValidator imageContentValidator,
      IImageExtensionValidator imageExtensionValidator,
      IUserRepository userRepository,
      IImageRepository imageRepository)
    {
      RuleFor(request => request)
        .MustAsync(async (request, _) => await userRepository.GetAsync(request.UserId) != null)
        .WithMessage("User doesn't exist.")
        .Must(request => request.ImageId != null || (!string.IsNullOrEmpty(request.Content) && !string.IsNullOrEmpty(request.Extension)))
        .WithMessage("ImageId or image content and extension must not be null.");

      When(request => request.ImageId != null,
        () =>
        {
          RuleFor(request => request.ImageId)
            .MustAsync(async (imageId, _) => await imageRepository.GetAsync(new List<Guid>() { imageId.Value }) != null)
            .WithMessage("Image with sended imageId doesn't exist");
        });

      When(request => !string.IsNullOrEmpty(request.Content) && !string.IsNullOrEmpty(request.Extension),
        () =>
        {
          RuleFor(request => request.Content)
            .SetValidator(imageContentValidator);
          RuleFor(request => request.Extension)
            .SetValidator(imageExtensionValidator);
        });
    }
  }
}
