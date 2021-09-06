using FluentValidation;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Validation.Communication.Interfaces;
using LT.DigitalOffice.UserService.Validation.User.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LT.DigitalOffice.UserService.Validation.User
{
  public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>, ICreateUserRequestValidator
  {
    private static Regex NumberRegex = new(@"\d");
    private static Regex SpecialCharactersRegex = new(@"[$&+,:;=?@#|<>.^*()%!]");
    private static Regex SpaceRegex = new(@"^[^@\s]*$");
    private static Regex NameRegex = new(@"^[a-zA-Zа-яА-ЯёЁ'][a-zA-Z-а-яА-ЯёЁ' ]+[a-zA-Zа-яА-ЯёЁ']?$");

    private readonly List<string> imageFormats = new()
    {
      ".jpg",
      ".jpeg",
      ".png",
      ".bmp",
      ".gif",
      ".tga"
    };

    public CreateUserRequestValidator(ICreateCommunicationRequestValidator communicationValidator)
    {
      RuleFor(user => user.FirstName)
          .NotEmpty()
          .WithMessage("First name cannot be empty.")
          .Must(x => !NumberRegex.IsMatch(x))
          .WithMessage("First name must not contain numbers.")
          .Must(x => !SpecialCharactersRegex.IsMatch(x))
          .WithMessage("First name must not contain special characters.")
          .MaximumLength(32)
          .WithMessage("First name is too long.")
          .Must(x => NameRegex.IsMatch(x.Trim()))
          .WithMessage("First name contains invalid characters.");

      RuleFor(user => user.LastName)
          .NotEmpty()
          .WithMessage("Last name cannot be empty.")
          .Must(x => !NumberRegex.IsMatch(x))
          .WithMessage("Last name must not contain numbers.")
          .Must(x => !SpecialCharactersRegex.IsMatch(x))
          .WithMessage("Last name must not contain special characters.")
          .MaximumLength(32)
          .WithMessage("Last name is too long.")
          .Must(x => NameRegex.IsMatch(x.Trim()))
          .WithMessage("Last name contains invalid characters.");

      RuleFor(user => user.PositionId)
          .NotEmpty();

      When(
          user => !string.IsNullOrEmpty(user.MiddleName),
          () =>
              RuleFor(user => user.MiddleName)
                  .Must(x => !NumberRegex.IsMatch(x))
                  .WithMessage("Middle name must not contain numbers.")
                  .Must(x => !SpecialCharactersRegex.IsMatch(x))
                  .WithMessage("Middle name must not contain special characters.")
                  .MaximumLength(32)
                  .WithMessage("Middle name is too long.")
                  .Must(x => NameRegex.IsMatch(x.Trim()))
                  .WithMessage("Middle name contains invalid characters."));

      When(
          user => !string.IsNullOrEmpty(user.City),
          () =>
              RuleFor(user => user.City)
                  .Must(x => !NumberRegex.IsMatch(x))
                  .WithMessage("City name must not contain numbers.")
                  .Must(x => !SpecialCharactersRegex.IsMatch(x))
                  .WithMessage("City name must not contain special characters.")
                  .MaximumLength(32)
                  .WithMessage("City name is too long.")
                  .Must(x => NameRegex.IsMatch(x.Trim()))
                  .WithMessage("City name contains invalid characters."));

      When(
        user => (user.AvatarImage != null),
        () =>
          RuleFor(user => user.AvatarImage)
            .Must(x => !string.IsNullOrEmpty(x.Content))
            .WithMessage("Content can't be empty")
            .Must(x => imageFormats.Contains(x.Extension))
            .WithMessage("Wrong extension")
        );

      RuleFor(user => user.Gender)
          .IsInEnum()
          .WithMessage("Wrong gender value.");

      RuleFor(user => user.Status)
          .IsInEnum()
          .WithMessage("Wrong status value.");

      RuleFor(user => user.Communications)
          .NotEmpty();

      RuleForEach(user => user.Communications)
          .SetValidator(communicationValidator);

      RuleFor(user => user.Rate)
          .GreaterThan(0)
          .LessThanOrEqualTo(1);

      When(user => user.Password != null && user.Password.Trim().Any(), () =>
      {
        RuleFor(user => user.Password.Trim())
            .MinimumLength(5)
            .WithMessage("Password is too short.")
            .Must(x => SpaceRegex.IsMatch(x))
            .WithMessage("Password must not contain space.");
      });
    }
  }
}