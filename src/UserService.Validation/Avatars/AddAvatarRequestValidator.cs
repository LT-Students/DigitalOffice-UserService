using FluentValidation;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Avatars;
using LT.DigitalOffice.UserService.Validation.Avatars.Interfaces;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Validation.Avatars
{
  public class AddAvatarRequestValidator : AbstractValidator<AddAvatarRequest>, IAddAvatarRequestValidator
  {
    private readonly List<string> imageFormats = new()
    {
      ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tga"
    };

    public AddAvatarRequestValidator(
      IUserRepository userRepository)
    {
      RuleFor(x => x.UserId)
        .Must(x => userRepository.Get(x) != null).WithMessage("User doesn't exist");

      RuleForEach(x => x.Images)
        .Must(x => !string.IsNullOrEmpty(x.Content)).WithMessage("Content can't be empty")
        .Must(x =>
        {
          try
          {
            var byteString = new Span<byte>(new byte[x.Content.Length]);
            return Convert.TryFromBase64String(x.Content, byteString, out _);
          }
          catch
          {
            return false;
          }
        }).WithMessage("Wrong image content.")
        .Must(x => imageFormats.Contains(x.Extension)).WithMessage("Wrong extension");
    }
  }
}
