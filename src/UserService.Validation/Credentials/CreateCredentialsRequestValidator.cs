﻿using FluentValidation;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials;
using LT.DigitalOffice.UserService.Validation.Credentials.Interfaces;
using System.Text.RegularExpressions;

namespace LT.DigitalOffice.UserService.Validation.Credentials
{
  public class CreateCredentialsRequestValidator : AbstractValidator<CreateCredentialsRequest>, ICreateCredentialsRequestValidator
  {
    private static Regex loginRegex = new(@"^([a-zA-Z]+)$|^([a-zA-Z0-9]*[0-9]+[a-zA-Z]+[0-9]*)$|^([a-zA-Z]+[0-9]+)$");

    public CreateCredentialsRequestValidator(
      IPendingUserRepository repository,
      IUserCredentialsRepository credentialsRepository)
    {
      RuleFor(request => request.Login.Trim())
        .MinimumLength(5).WithMessage("Login is too short.")
        .MaximumLength(15).WithMessage("Login is too long.")
        .Must(login => loginRegex.IsMatch(login))
        .WithMessage("Login must contain only Latin letters and digits or only Latin letters.");

      RuleFor(request => request.UserId)
        .NotEmpty().WithMessage("UserId can't be empty.");

      RuleFor(request => request)
        .Cascade(CascadeMode.Stop)
        .MustAsync(async (r, _) => !await credentialsRepository.DoesExistAsync(r.UserId))
        .WithMessage("The credentials already exist.")
        .MustAsync(async (r, _) => !await credentialsRepository.DoesLoginExistAsync(r.Login))
        .WithMessage("The login already exist.")
        .MustAsync(async (r, _) =>
          (await repository.GetAsync(r.UserId))?.Password == r.Password)
        .WithMessage("The wrong password.");
    }
  }
}
