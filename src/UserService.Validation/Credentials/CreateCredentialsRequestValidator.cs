using FluentValidation;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials;
using LT.DigitalOffice.UserService.Validation.Credentials.Interfaces;
using LT.DigitalOffice.UserService.Validation.Credentials.Resources;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;

namespace LT.DigitalOffice.UserService.Validation.Credentials
{
  public class CreateCredentialsRequestValidator : AbstractValidator<CreateCredentialsRequest>, ICreateCredentialsRequestValidator
  {
    private static Regex loginRegex = new(@"^([a-zA-Z]+)$|^([a-zA-Z0-9]*[0-9]+[a-zA-Z]+[0-9]*)$|^([a-zA-Z]+[0-9]+)$");

    public CreateCredentialsRequestValidator(
      IPendingUserRepository repository,
      IUserCredentialsRepository credentialsRepository)
    {
      Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");

      RuleFor(request => request.Login.Trim())
        .MinimumLength(5).WithMessage(CreateCredentialsRequestValidationResource.LoginShort)
        .MaximumLength(15).WithMessage(CreateCredentialsRequestValidationResource.LoginLong)
        .Must(login => loginRegex.IsMatch(login))
        .WithMessage(CreateCredentialsRequestValidationResource.LoginMatch);

      RuleFor(request => request.UserId)
        .NotEmpty().WithMessage(CreateCredentialsRequestValidationResource.UserId);

      RuleFor(request => request)
        .Cascade(CascadeMode.Stop)
        .MustAsync(async (r, _) => !await credentialsRepository.DoesExistAsync(r.UserId))
        .WithMessage(CreateCredentialsRequestValidationResource.CredentialsExist)
        .MustAsync(async (r, _) => !await credentialsRepository.DoesLoginExistAsync(r.Login))
        .WithMessage(CreateCredentialsRequestValidationResource.LoginExist)
        .MustAsync(async (r, _) =>
          (await repository.GetAsync(r.UserId))?.Password == r.Password)
        .WithMessage(CreateCredentialsRequestValidationResource.PasswordWrong);
    }
  }
}
