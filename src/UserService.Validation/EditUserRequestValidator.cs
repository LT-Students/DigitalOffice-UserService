using FluentValidation;
using LT.DigitalOffice.UserService.Models.Dto;

namespace LT.DigitalOffice.UserService.Validation
{
    public class EditUserRequestValidator : AbstractValidator<EditUserRequest>
    {
        public EditUserRequestValidator()
        {
            RuleFor(user => user.Id)
                .NotEmpty();

            RuleFor(user => user.FirstName)
                .NotEmpty()
                .MaximumLength(32).WithMessage("First name is too long.")
                .MinimumLength(1).WithMessage("First name is too short.")
                .Matches("^[A-Z][a-z]+$|^[А-ЯЁ][а-яё]+$").WithMessage("First name with error.");

            RuleFor(user => user.LastName)
                .NotEmpty()
                .MaximumLength(32).WithMessage("Last name is too long.")
                .MinimumLength(1).WithMessage("Last name is too short.")
                .Matches("^[A-Z][a-z]+$|^[А-ЯЁ][а-яё]+$").WithMessage("Last name with error.");

            When(
                user => !string.IsNullOrEmpty(user.MiddleName),
                () =>
                    RuleFor(user => user.MiddleName)
                        .MaximumLength(32).WithMessage("Middle name is too long.")
                        .MinimumLength(1).WithMessage("Middle name is too short.")
                        .Matches("^[A-Z][a-z]+$|^[А-ЯЁ][а-яё]+$").WithMessage("Middle name with error."));

            RuleFor(user => user.Email)
                .NotEmpty()
                .MaximumLength(254).WithMessage("Email is too long.")
                .EmailAddress().WithMessage("Email is invalid.");

            RuleFor(user => user.Status)
                .MaximumLength(300).WithMessage("Status is too long.");

            RuleFor(user => user.Password)
                .NotEmpty();
        }
    }
}