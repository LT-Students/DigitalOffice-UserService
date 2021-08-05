using FluentValidation;
using LT.DigitalOffice.UserService.Validation.Login.Interfaces;
using System.Linq;

namespace LT.DigitalOffice.UserService.Validation.Login
{
    public class LoginValidator : AbstractValidator<string>, ILoginValidator
    {
        public LoginValidator()
        {
            RuleFor(login => login)
                .NotEmpty()
                .WithMessage("Login can't be empty");
            When(login => !string.IsNullOrEmpty(login),
            () =>
            {
                RuleFor(login => login)
                .Must(x => char.IsLetter(x[0]))
                .WithMessage("Login must start with letter")
                .MinimumLength(3)
                .WithMessage("Login is too short")
                .MaximumLength(15)
                .WithMessage("Login is too long")
                .Must(x => x.All(char.IsLetterOrDigit))
                .WithMessage("Login must contain only letters or digits");
            });
        }
    }
}
