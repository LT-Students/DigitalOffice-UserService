using FluentValidation;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Validation.Image.Interfaces;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Validation.Image
{
  public class EditAvatarValidator : AbstractValidator<(Guid userId, Guid imageId)>, IEditAvatarRequestValidator
  {
    public EditAvatarValidator(
      IUserRepository userRepository,
      IImageRepository imageRepository)
    {
      RuleFor(request => request.userId)
        .MustAsync(async (userId, _) => await userRepository.GetAsync(userId) != null)
        .WithMessage("User doesn't exist.");

      RuleFor(request => request.imageId)
        .MustAsync(async (imageId, _) => await imageRepository.GetAsync(new List<Guid>() { imageId }) != null)
        .WithMessage("Image with sended imageId doesn't exist");
    }
  }
}
