using FluentValidation;
using LT.DigitalOffice.UserService.Validation.Email.Interfaces;

namespace LT.DigitalOffice.UserService.Validation.Email
{
    public class EmailValidator : AbstractValidator<string>, IEmailValidator
    {
        public EmailValidator()
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
