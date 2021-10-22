﻿using FluentValidation;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Communication;
using LT.DigitalOffice.UserService.Validation.Communication.Interfaces;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace LT.DigitalOffice.UserService.Validation.Communication
{
  public class CreateCommunicationRequestValidator : AbstractValidator<CreateCommunicationRequest>, ICreateCommunicationRequestValidator
  {
    private static Regex PhoneRegex = new(@"^\d+$");

    public CreateCommunicationRequestValidator(
      ICommunicationRepository _communicationRepository)
    {
      RuleFor(c => c.Value)
        .NotEmpty().WithMessage("Communication value must not be empty.");

      RuleFor(c => c.Type)
        .IsInEnum().WithMessage("Incorrect communication type format.");

      When(c => c.Type == CommunicationType.Phone && c.Value != null, () =>
        RuleFor(c => c.Value)
          .Must(v => PhoneRegex.IsMatch(v.Trim())).WithMessage("Incorrect phone number."));

      When(c => c.Type == CommunicationType.Email && c.Value != null, () =>
        RuleFor(c => c.Value)
          .Must(v =>
          {
            try
            {
              MailAddress address = new(v?.Trim());
              return true;
            }
            catch
            {
              return false;
            }
          })
          .WithMessage("Incorrect email address."));

      RuleFor(c => c.Value)
        .MustAsync(async (v, _, _) => !await _communicationRepository.CheckExistingValue(v.Value))
        .WithMessage("Communication value already exist.");
    }
  }
}
