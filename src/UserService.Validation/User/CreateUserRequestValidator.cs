using FluentValidation;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Validation.Communication.Interfaces;
using LT.DigitalOffice.UserService.Validation.Image.Interfaces;
using LT.DigitalOffice.UserService.Validation.Password.Interfaces;
using LT.DigitalOffice.UserService.Validation.User.Interfaces;
using System.Text.RegularExpressions;

namespace LT.DigitalOffice.UserService.Validation.User
{
  public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>, ICreateUserRequestValidator
  {
    private static Regex NumberRegex = new(@"\d");
    private static Regex SpecialCharactersRegex = new(@"[$&+,:;=?@#|<>.^*()%!]");
    private static Regex NameRegex = new(@"^([a-zA-Zа-яА-ЯёЁ]+|[a-zA-Zа-яА-ЯёЁ]+[-|']?[a-zA-Zа-яА-ЯёЁ]+|[a-zA-Zа-яА-ЯёЁ]+[-|']?[a-zA-Zа-яА-ЯёЁ]+[-|']?[a-zA-Zа-яА-ЯёЁ]+)$");

    public CreateUserRequestValidator(
      ICreateCommunicationRequestValidator communicationValidator,
      ICreateAvatarRequestValidator imageValidator,
      IPasswordValidator passwordValidator)
    {
      RuleFor(user => user.FirstName)
        .NotEmpty().WithMessage("First name cannot be empty.")
        .Must(x => !NumberRegex.IsMatch(x))
        .WithMessage("First name must not contain numbers.")
        .Must(x => !SpecialCharactersRegex.IsMatch(x))
        .WithMessage("First name must not contain special characters.")
        .MaximumLength(45).WithMessage("First name is too long.")
        .Must(x => NameRegex.IsMatch(x.Trim()))
        .WithMessage("First name contains invalid characters.");

      RuleFor(user => user.LastName)
        .NotEmpty().WithMessage("Last name cannot be empty.")
        .Must(x => !NumberRegex.IsMatch(x))
        .WithMessage("Last name must not contain numbers.")
        .Must(x => !SpecialCharactersRegex.IsMatch(x))
        .WithMessage("Last name must not contain special characters.")
        .MaximumLength(45).WithMessage("Last name is too long.")
        .Must(x => NameRegex.IsMatch(x.Trim()))
        .WithMessage("Last name contains invalid characters.");

      When(
        user => !string.IsNullOrEmpty(user.MiddleName),
        () =>
          RuleFor(user => user.MiddleName)
            .Must(x => !NumberRegex.IsMatch(x))
            .WithMessage("Middle name must not contain numbers.")
            .Must(x => !SpecialCharactersRegex.IsMatch(x))
            .WithMessage("Middle name must not contain special characters.")
            .MaximumLength(45).WithMessage("Middle name is too long.")
            .Must(x => NameRegex.IsMatch(x.Trim()))
            .WithMessage("Middle name contains invalid characters."));

      When(
        user => (user.AvatarImage != null),
        () =>
          RuleFor(user => user.AvatarImage)
            .SetValidator(imageValidator)
        );

      RuleFor(user => user.Status)
        .IsInEnum().WithMessage("Wrong status value.");

      When(user => user.About != null, () =>
      {
        RuleFor(user => user.About)
          .MaximumLength(150).WithMessage("About is too long.");
      });

      RuleFor(user => user.Communications)
        .NotEmpty();

      RuleForEach(user => user.Communications)
        .SetValidator(communicationValidator);

      RuleFor(user => user.Rate)
        .GreaterThan(0)
        .LessThanOrEqualTo(1);

      When(
        user => (!string.IsNullOrEmpty(user.Password)),
        () =>
          RuleFor(user => user.Password)
            .SetValidator(passwordValidator)
        );
    }
  }
}