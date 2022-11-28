using FluentValidation;
using LT.DigitalOffice.UserService.Validation.Password.Interfaces;
using System.Text.RegularExpressions;

namespace LT.DigitalOffice.UserService.Validation.Password
{
  public class PasswordValidator : AbstractValidator<string>, IPasswordValidator
  {
    private static Regex PasswordRegex = new(@"(?=.*[.,:;?!*+%\-<>@[\]{}/\\_{}$#])");

    public PasswordValidator()
    {
      RuleFor(p => p)
        .MinimumLength(8).WithMessage("Password is too short.")
        .MaximumLength(50).WithMessage("Password is too long.")
        .Must(p => PasswordRegex.IsMatch(p))
        .WithMessage("The password must contain at least one special character.")
        .Must(p => !p.Contains(' ')).WithMessage("Password must not contain space.");
    }
  }
}
