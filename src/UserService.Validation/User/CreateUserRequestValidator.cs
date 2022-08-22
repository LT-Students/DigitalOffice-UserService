using FluentValidation;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Validation.Communication.Interfaces;
using LT.DigitalOffice.UserService.Validation.Image.Interfaces;
using LT.DigitalOffice.UserService.Validation.Password.Interfaces;
using LT.DigitalOffice.UserService.Validation.User.Interfaces;
using System;
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
        .Must(x => !NumberRegex.IsMatch(x))
        .WithMessage("First name must not contain numbers.")
        .Must(x => !SpecialCharactersRegex.IsMatch(x))
        .WithMessage("First name must not contain special characters.")
        .MaximumLength(45).WithMessage("First name is too long.")
        .Must(x => NameRegex.IsMatch(x.Trim()))
        .WithMessage("First name contains invalid characters.");

      RuleFor(user => user.LastName)
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

      When(user => user.About is not null, () =>
      {
        RuleFor(user => user.About)
          .MaximumLength(150).WithMessage("About is too long.");
      });

      RuleFor(user => user.Communication)
        .Cascade(CascadeMode.Stop)
        .Must(x => x.Type == CommunicationType.Email)
        .WithMessage("Communication type must be email.")
        .SetValidator(communicationValidator);

      When(user => user.UserCompany is not null && user.UserCompany.Rate is not null, () =>
      {
        RuleFor(user => user.UserCompany.Rate)
          .GreaterThan(0)
          .LessThanOrEqualTo(1);

        RuleFor(user => user.UserCompany.ContractTermType)
          .Must(x => Enum.IsDefined(typeof(ContractTerm), x))
          .WithMessage("Wrong contract term type.");
      });

      When(
        user => (!string.IsNullOrEmpty(user.Password)),
        () =>
          RuleFor(user => user.Password)
            .SetValidator(passwordValidator)
        );
    }
  }
}