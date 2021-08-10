using FluentValidation;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Validation.User.Interfaces;
using System.Linq;
using System.Text.RegularExpressions;

namespace LT.DigitalOffice.UserService.Validation.User
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>, ICreateUserRequestValidator
    {
        private static Regex NumberRegex = new (@"\d");
        private static Regex SpecialCharactersRegex = new (@"[$&+,:;=?@#|'<>.^*()%!-]");
        private static Regex SpaceRegex = new (@"^[^@\s]*$");
        private static Regex EmailRegex = new (@"^[^@\s]+@[^@\s]+\.[^@\s]*$");

        public CreateUserRequestValidator()
        {
            RuleFor(user => user.FirstName)
                .NotEmpty()
                .WithMessage("The first name cannot be empty")
                .Must(x => !NumberRegex.IsMatch(x))
                .WithMessage("First name must not contain numbers")
                .Must(x => !SpecialCharactersRegex.IsMatch(x))
                .WithMessage("First name must not contain special characters")
                .MaximumLength(32)
                .WithMessage("First name is too long.")
                .Must(x => SpaceRegex.IsMatch(x.Trim()));

            RuleFor(user => user.LastName)
                .NotEmpty()
                .WithMessage("The last name cannot be empty")
                .Must(x => !NumberRegex.IsMatch(x))
                .WithMessage("Last name must not contain numbers")
                .Must(x => !SpecialCharactersRegex.IsMatch(x))
                .WithMessage("Last name must not contain special characters")
                .MaximumLength(32)
                .WithMessage("Last name is too long.")
                .Must(x => SpaceRegex.IsMatch(x.Trim()));

            RuleFor(user => user.PositionId)
                .NotEmpty();

            When(
                user => !string.IsNullOrEmpty(user.MiddleName),
                () =>
                    RuleFor(user => user.MiddleName)
                        .Must(x => !NumberRegex.IsMatch(x))
                        .WithMessage("Middle name must not contain numbers")
                        .Must(x => !SpecialCharactersRegex.IsMatch(x))
                        .WithMessage("Middle name must not contain special characters")
                        .MaximumLength(32)
                        .WithMessage("Middle name is too long.")
                        .Must(x => SpaceRegex.IsMatch(x.Trim())));

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
                        c.RuleFor(uc => uc.Value)
                        .NotEmpty()
                        .WithMessage("The email cannot be empty")
                        .Must(x => EmailRegex.IsMatch(x))
                        .WithMessage("Incorrect email address.");
                        c.RuleFor(uc => uc.UserId).Null();
                    });
            });

            RuleFor(user => user.Rate)
                .GreaterThan(0)
                .LessThanOrEqualTo(1);

            RuleFor(user => user.Password)
                .NotEmpty()
                .WithMessage("The password cannot be empty")
                .MinimumLength(5)
                .WithMessage("Password is too short.")
                .Must(x => SpaceRegex.IsMatch(x))
                .WithMessage("Password must not contain space.");


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