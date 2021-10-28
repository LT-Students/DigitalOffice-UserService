using FluentValidation;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Validation.Password.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace LT.DigitalOffice.UserService.Validation.Password
{
  public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>, IChangePassordRequestValidator
  {
    public ChangePasswordRequestValidator(
      IMemoryCache cache)
    {
      RuleFor(r => r.UserId)
        .NotEmpty().WithMessage("User id must not be empty.");

      RuleFor(r => r.Secret)
        .NotEmpty().WithMessage("Secret must not be empty.");

      RuleFor(r => r.Login)
        .NotEmpty().WithMessage("Login must not be empty.");

      RuleFor(r => r.NewPassword)
        .NotEmpty().WithMessage("Pussword must not be empty.");

      RuleFor(r => r)
        .Must(r => cache.TryGetValue(r.Secret, out Guid savedUserId) && savedUserId == r.UserId)
        .WithMessage("Invalid user data.");
    }
  }
}
