using FluentValidation;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Communication;
using LT.DigitalOffice.UserService.Validation.Communication.Interfaces;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace LT.DigitalOffice.UserService.Validation.Communication
{
  public class CreateCommunicationRequestValidator : AbstractValidator<CreateCommunicationRequest>, ICreateCommunicationRequestValidator
  {
    private static Regex PhoneRegex = new(@"^\d+$");

    public CreateCommunicationRequestValidator(
      IUserCommunicationRepository _communicationRepository)
    {
      RuleFor(c => c.Type)
        .IsInEnum()
        .WithMessage("Incorrect communication type format.")
        .Must(ct => ct != CommunicationType.BaseEmail)
        .WithMessage("Can't set unconfirmed email as base.");

      When(c => c.Type == CommunicationType.Phone, () =>
        RuleFor(c => c.Value)
          .Must(v => PhoneRegex.IsMatch(v.Trim())).WithMessage("Incorrect phone number."));

      When(c => c.Type == CommunicationType.Email, () =>
        RuleFor(c => c.Value)
          .Must(v =>
          {
            try
            {
              MailAddress address = new(v.Trim());
              return true;
            }
            catch
            {
              return false;
            }
          })
          .WithMessage("Incorrect email address."));

      RuleFor(c => c.Value)
        .MustAsync(async (v, _, _) => !await _communicationRepository.DoesValueExist(v.Value))
        .WithMessage("Communication value already exist.");

      RuleFor(c => c.VisibleTo)
        .IsInEnum()
        .WithMessage("Wrong visible to value.");
    }
  }
}
