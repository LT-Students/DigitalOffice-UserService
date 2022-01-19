using FluentValidation;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials;
using LT.DigitalOffice.UserService.Validation.Credentials.Interfaces;
using System.Linq;

namespace LT.DigitalOffice.UserService.Validation.Credentials
{
  public class CreateCredentialsRequestValidator : AbstractValidator<CreateCredentialsRequest>, ICreateCredentialsRequestValidator
  {
    public CreateCredentialsRequestValidator(
      IPendingUserRepository repository,
      IUserCredentialsRepository credentialsRepository)
    {
      RuleFor(request => request.Login.Trim() )
        .Cascade(CascadeMode.Stop)
        .NotEmpty().WithMessage("Login can't be empty.")
        .Must(login => char.IsLetter(login[0])).WithMessage("Login must start with letter.")
        .MinimumLength(3).WithMessage("Login is too short.")
        .MaximumLength(15).WithMessage("Login is too long")
        .Must(login => login.All(char.IsLetterOrDigit))
        .WithMessage("Login must contain only letters or digits.");

      RuleFor(request => request.UserId)
        .NotEmpty().WithMessage("UserId can't be empty.");

      RuleFor(request => request.Password)
        .NotEmpty().WithMessage("Password can't be empty.");

      RuleFor(request => request)
        .Cascade(CascadeMode.Stop)
        .MustAsync(async (r, _) => !await credentialsRepository.CredentialsExistAsync(r.UserId))
        .WithMessage("The credentials already exist.")
        .MustAsync(async (r, _) => !await credentialsRepository.LoginExistAsync(r.Login))
        .WithMessage("The login already exist.")
        .MustAsync(async (r, _) =>
          (await repository.GetAsync(r.UserId))?.Password == r.Password)
        .WithMessage("The wrong password.");
    }
  }
}
