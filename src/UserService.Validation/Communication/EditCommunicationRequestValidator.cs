using FluentValidation;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Communication;
using LT.DigitalOffice.UserService.Validation.Communication.Interfaces;

namespace LT.DigitalOffice.UserService.Validation.Communication
{
  public class EditCommunicationRequestValidator : AbstractValidator<(
      DbUserCommunication dbUserCommunication,
      EditCommunicationRequest request)>,
    IEditCommunicationRequestValidator
  {
    private readonly IUserCommunicationRepository _communicationRepository;

    public EditCommunicationRequestValidator(
      IUserCommunicationRepository communicationRepository)
    {
      _communicationRepository = communicationRepository;

      RuleFor(x => x)
        .Must(x => !(x.request.Type is not null && !string.IsNullOrEmpty(x.request.Value)))
        .WithMessage("it is not allowed to change more than 2 properties at the same time.")
        .Must(x => !(x.request.Type is null && string.IsNullOrEmpty(x.request.Value)))
        .WithMessage("No change to apply.");

      When(x => x.request.Type is not null, () =>
      {
        RuleFor(x => x)
          .Cascade(CascadeMode.Stop)
          .Must(x => x.request.Type == CommunicationType.BaseEmail)
          .WithMessage("Invalid type")
          .Must(x => x.dbUserCommunication.IsConfirmed
            && x.dbUserCommunication.Type == (int)CommunicationType.Email)
          .WithMessage("Only a verified email can be set as the base email.");
      });

      When(x => !string.IsNullOrEmpty(x.request.Value), () =>
      {
        RuleFor(x => x.request.Value)
          .MustAsync(async (x, _) => !await _communicationRepository.DoesValueExist(x))
          .WithMessage("Communication value already exist.");
      });

      When(x => x.request.VisibleTo is not null, () =>
      {
        RuleFor(x => x.request.VisibleTo)
          .IsInEnum()
          .WithMessage("Wrong visibly to value");
      });
    }
  }
}
