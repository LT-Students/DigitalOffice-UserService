using FluentValidation;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Achievement;
using LT.DigitalOffice.UserService.Validation.Achievement.Interfaces;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Validation.Achievement
{
  public class CreateAchievementRequestValidator : AbstractValidator<CreateAchievementRequest>, ICreateAchievementRequestValidator
  {
    private List<string> AllowedExtensions = new()
    { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tga" };

    public CreateAchievementRequestValidator()
    {
      RuleFor(x => x.Name)
        .NotEmpty().WithMessage("Name of Achievement not must be empty")
        .MaximumLength(100)
        .WithMessage("Name of Achievement is too long");

      RuleFor(x => x.Description)
        .NotEmpty().WithMessage("Description of Achievement not must be empty")
        .MaximumLength(1000)
        .WithMessage("Description of Achievement is too long");

      When(w => w.Image != null, () =>
      {
        RuleFor(w => w.Image.Content)
          .NotEmpty().WithMessage("Image content cannot be empty.")
          .Must(x =>
          {
            try
            {
              var byteString = new Span<byte>(new byte[x.Length]);
              return Convert.TryFromBase64String(x, byteString, out _);
            }
            catch
            {
              return false;
            }
          }).WithMessage("Wrong image content.");

        RuleFor(w => w.Image.Extension)
          .Must(AllowedExtensions.Contains)
          .WithMessage($"Image extension is not {string.Join('/', AllowedExtensions)}");
      });
    }
  }
}
