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

      RuleFor(r => r.NewPassword)
        .SetValidator(passwordValidator);

      RuleFor(r => r)
        .Must(r => cache.TryGetValue(r.Secret, out Guid savedUserId) && savedUserId == r.UserId)
        .WithMessage("Invalid user data.");
    }
  }
}
