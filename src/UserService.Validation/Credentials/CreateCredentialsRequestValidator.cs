using FluentValidation;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials;
using LT.DigitalOffice.UserService.Validation.Credentials.Interfaces;
using System.Linq;

namespace LT.DigitalOffice.UserService.Validation.Credentials
{
    public class CreateCredentialsRequestValidator : AbstractValidator<CreateCredentialsRequest>, ICreateCredentialsRequestValidator
    {
        public CreateCredentialsRequestValidator()
        {
            RuleFor(request => request.Login)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Login can't be empty")
                .Must(login => char.IsLetter(login[0]))
                .WithMessage("Login must start with letter")
                .MinimumLength(3)
                .WithMessage("Login is too short")
                .MaximumLength(15)
                .WithMessage("Login is too long")
                .Must(login => login.All(char.IsLetterOrDigit))
                .WithMessage("Login must contain only letters or digits");
            RuleFor(request => request.UserId)
                .NotEmpty()
                .WithMessage("UserId can't be empty");
            RuleFor(request => request.Password)
                .NotEmpty()
                .WithMessage("Password can't be empty");
        }
    }
}
