using FluentValidation;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Communication;
using LT.DigitalOffice.UserService.Validation.Communication.Interfaces;
using LT.DigitalOffice.UserService.Validation.Communication.Resources;
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
        .WithMessage(CreateCommunicationRequestValidatorResource.IncorrectCommunicationTypeFormat)
        .Must(ct => ct != CommunicationType.BaseEmail)
        .WithMessage(CreateCommunicationRequestValidatorResource.UnconfirmedEmail);
       
      When(c => c.Type == CommunicationType.Phone, () =>
        RuleFor(c => c.Value)
          .Must(v => PhoneRegex.IsMatch(v.Trim())).WithMessage(CreateCommunicationRequestValidatorResource.IncorrectPhoneNumber));

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
          .WithMessage(CreateCommunicationRequestValidatorResource.IncorrectEmailAddress));

      RuleFor(c => c.Value)
        .MustAsync(async (v, _, _) => !await _communicationRepository.DoesValueExist(v.Value))
        .WithMessage(CreateCommunicationRequestValidatorResource.ExistingCommunicationValue);
    }
  }
}
