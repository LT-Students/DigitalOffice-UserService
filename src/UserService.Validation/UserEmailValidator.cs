using FluentValidation;

namespace LT.DigitalOffice.UserService.Validation
{
    public class UserEmailValidator : AbstractValidator<string>
    {
        public UserEmailValidator()
        {
            RuleFor(email => email)
                .NotEmpty()
                .MaximumLength(129)
                .WithMessage("Email is too long.")
                .EmailAddress()
                .WithMessage("Email is invalid.");
        }
    }
}