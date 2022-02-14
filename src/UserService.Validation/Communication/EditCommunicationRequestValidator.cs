using FluentValidation;
using FluentValidation.Validators;
using LT.DigitalOffice.Kernel.Validators;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Communication;
using LT.DigitalOffice.UserService.Validation.Communication.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Validation.Communication
{
  public class EditCommunicationRequestValidator :
    ExtendedEditRequestValidator<DbUserCommunication, EditCommunicationRequest>, IEditCommunicationRequestValidator
  {
    private readonly ICommunicationRepository _communicationRepository;

    private async Task HandleInternalPropertyValidation(
      (DbUserCommunication communication, JsonPatchDocument<EditCommunicationRequest> patch) request,
      CustomContext context)
    {
      RequestedOperation = request.patch.Operations[0];
      Context = context;

      #region Paths

      AddСorrectPaths(
        new List<string>
        {
          nameof(EditCommunicationRequest.Type),
          nameof(EditCommunicationRequest.Value)
        });

      AddСorrectOperations(nameof(EditCommunicationRequest.Type), new() { OperationType.Replace });
      AddСorrectOperations(nameof(EditCommunicationRequest.Value), new() { OperationType.Replace });

      #endregion

      #region Value

      AddFailureForPropertyIf(
        nameof(EditCommunicationRequest.Value),
        x => x == OperationType.Replace,
        new()
        {
          { x => !string.IsNullOrEmpty(x.value.ToString()), "Communication value must not be empty." }
        });

      await AddFailureForPropertyIfAsync(
        nameof(EditCommunicationRequest.Value),
        x => x == OperationType.Replace,
        new()
        {
          {
            async x =>
            !await _communicationRepository.DoesValueExist(x.value.ToString()),
            "Communication value already exist."
          }
        });

      #endregion

      #region Types

      AddFailureForPropertyIf(
        nameof(EditCommunicationRequest.Type),
        x => x == OperationType.Replace,
        new()
        {
          {
            x => Enum.TryParse(x.value.ToString(), true, out CommunicationType result)
              && result == CommunicationType.BaseEmail,
            "Incorrect format of communication type."
          },
          {
            x => request.communication.IsConfirmed
              && request.communication.Type == (int)CommunicationType.Email,
            "Only a verified email can be set as the base email."
          }
        }, CascadeMode.Stop);

      #endregion
    }

    public EditCommunicationRequestValidator(
      ICommunicationRepository communicationRepository)
    {
      _communicationRepository = communicationRepository;

      RuleFor(request => request)
        .Must(request => request.Item2.Operations.Count() == 1)
        .WithMessage("it is not allowed to change more than 2 properties at the same time.")
        .CustomAsync(async (request, context, _) => await HandleInternalPropertyValidation(request, context));
    }
  }
}
