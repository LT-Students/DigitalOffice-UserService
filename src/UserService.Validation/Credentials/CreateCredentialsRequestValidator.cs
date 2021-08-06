using FluentValidation;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials;
using LT.DigitalOffice.UserService.Validation.Credentials.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Validation.Credentials
{
    public class CreateCredentialsRequestValidator : AbstractValidator<CreateCredentialsRequest>, ICreateCredentialsRequestValidator
    {
        public CreateCredentialsRequestValidator()
        {
            RuleFor(request => request.Login)
                .NotEmpty()
                .WithMessage("Login can't be empty");
            RuleFor(request => request.UserId)
                .NotEmpty()
                .WithMessage("UserId can't be empty");
            RuleFor(request => request.Password)
                .NotEmpty()
                .WithMessage("Password can't be empty"); 
            When(request => !string.IsNullOrEmpty(request.Login),
            () =>
            {
                RuleFor(request => request.Login)
                    .Must(login => char.IsLetter(login[0]))
                    .WithMessage("Login must start with letter")
                    .MinimumLength(3)
                    .WithMessage("Login is too short")
                    .MaximumLength(15)
                    .WithMessage("Login is too long")
                    .Must(login => login.All(char.IsLetterOrDigit))
                    .WithMessage("Login must contain only letters or digits");
            });
        }
    }
}
