using FluentValidation;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Validation.Image.Interfaces;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Validation.Image
{
  public class AddImageRequestValidator : AbstractValidator<AddImageRequest>, IAddImageRequestValidator
  {
    private readonly List<string> imageFormats = new()
    {
      ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tga"
    };

    public AddImageRequestValidator()
    {
      RuleFor(x => x.Content)
        .NotEmpty().WithMessage("Content can't be empty")
        .Must(content =>
        {
          try
          {
            var byteString = new Span<byte>(new byte[content.Length]);
            return Convert.TryFromBase64String(content, byteString, out _);
          }
          catch
          {
            return false;
          }
        }).WithMessage("Wrong image content.");

      RuleFor(x => x.Extension)
        .Must(x => imageFormats.Contains(x)).WithMessage("Wrong extension");
    }
  }
}
