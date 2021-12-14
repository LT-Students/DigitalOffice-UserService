using FluentValidation;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Validation.Password.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace LT.DigitalOffice.UserService.Validation.Password
{
  public class ReconstructPasswordRequestValidator : AbstractValidator<ReconstructPasswordRequest>, IReconstructPassordRequestValidator
  {
    public ReconstructPasswordRequestValidator(
      IMemoryCache cache,
      IPasswordValidator passwordValidator)
    {
      RuleFor(r => r.UserId)
        .NotEmpty().WithMessage("User id must not be empty.");

      RuleFor(r => r.Secret)
        .NotEmpty().WithMessage("Secret must not be empty.");

      RuleFor(r => r.Login)
        .NotEmpty().WithMessage("Login must not be empty.");

      RuleFor(r => r.NewPassword)
        .Cascade(CascadeMode.Stop)
        .NotEmpty().WithMessage("New password must not be empty.")
        .SetValidator(passwordValidator);

      RuleFor(r => r)
        .Must(r => cache.TryGetValue(r.Secret, out Guid savedUserId) && savedUserId == r.UserId)
        .WithMessage("Invalid user data.");
    }
  }
}
