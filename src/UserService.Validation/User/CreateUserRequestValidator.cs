using FluentValidation;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Validation.User.Interfaces;
using System.Linq;
using System.Text.RegularExpressions;

namespace LT.DigitalOffice.UserService.Validation.User
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>, ICreateUserRequestValidator
    {
        private static Regex NameRegex = new(@"\d");
        public CreateUserRequestValidator()
        {
            RuleFor(user => user.FirstName)
                .NotEmpty()
                .Must(x => !NameRegex.IsMatch(x))
                .WithMessage("First name must not contain numbers")
                .MaximumLength(32)
                .WithMessage("First name is too long.");

            RuleFor(user => user.LastName)
                .NotEmpty()
                .Must(x => !NameRegex.IsMatch(x))
                .WithMessage("Last name must not contain numbers")
                .MaximumLength(32)
                .WithMessage("Last name is too long.");

            When(
                user => !string.IsNullOrEmpty(user.MiddleName),
                () =>
                    RuleFor(user => user.MiddleName)
                        .Must(x => !NameRegex.IsMatch(x))
                        .WithMessage("Middle name must not contain numbers")
                        .MaximumLength(32)
                        .WithMessage("Middle name is too long."));

            When(
                user => !string.IsNullOrEmpty(user.City),
                () =>
                    RuleFor(user => user.City)
                        .MaximumLength(32)
                        .WithMessage("City name is too long."));

            RuleFor(user => user.Gender)
                .IsInEnum()
                .WithMessage("Wrong gender value.");

            RuleFor(user => user.Status)
                .IsInEnum()
                .WithMessage("Wrong status value.");

            When(user => user.Communications != null && user.Communications.Any(), () =>
            {
                RuleForEach(user => user.Communications)
                    .ChildRules(c =>
                    {
                        c.RuleFor(uc => uc.Value).NotEmpty();
                        c.RuleFor(uc => uc.UserId).Null();
                    });
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