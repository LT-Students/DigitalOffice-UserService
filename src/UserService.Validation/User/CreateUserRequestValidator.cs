using FluentValidation;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Validation.User.Interfaces;
using System.Linq;

namespace LT.DigitalOffice.UserService.Validation.User
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>, ICreateUserRequestValidator
    {
        public CreateUserRequestValidator()
        {
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

            When(
                user => !string.IsNullOrEmpty(user.City),
                () =>
                    RuleFor(user => user.City)
                        .MaximumLength(32).WithMessage("City name is too long.")
                        .MinimumLength(1).WithMessage("City name is too short.")
                        .Matches("[A-Z][a-z]+$|[А-ЯЁ][а-яё]+$").WithMessage("City name with error."));

            RuleFor(user => user.Gender)
                .IsInEnum().WithMessage("Wrong gender value.");

            RuleFor(user => user.Status)
                .IsInEnum().WithMessage("Wrong status value.");

            When(user => user.Communications != null && user.Communications.Any(), () =>
            {
                RuleForEach(user => user.Communications).ChildRules(c => c.RuleFor(uc => uc.Value).NotEmpty());
            });

            RuleFor(user => user.Rate)
                .GreaterThan(0)
                .LessThanOrEqualTo(1);

            // TODO move to edit user validation
            //When(user => user.Skills != null && user.Skills.Any(), () =>
            //{
            //    RuleForEach(request => request.Skills)
            //        .NotEmpty()
            //        .MaximumLength(30).WithMessage("Skill name is too long");
            //});
        }
    }
}