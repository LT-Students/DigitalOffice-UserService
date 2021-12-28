using FluentValidation;
using FluentValidation.Validators;
using LT.DigitalOffice.Kernel.Validators;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Communication;
using LT.DigitalOffice.UserService.Validation.Communication.Interfaces;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Validation.Communication
{
  public class EditCommunicationRequestValidator :
    BaseEditRequestValidator<EditCommunicationRequest>, IEditCommunicationRequestValidator
  {
    private readonly ICommunicationRepository _communicationRepository;

    private async Task HandleInternalPropertyValidation(Operation<EditCommunicationRequest> requestedOperation, CustomContext context)
    {
      RequestedOperation = requestedOperation;
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
          { async x =>
            !await _communicationRepository.CheckExistingValue(x.value.ToString()),
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
            x =>
            Enum.TryParse(typeof(CommunicationType), x.value.ToString(), out _),
            "Incorrect format of communication type."
          }
        });

      #endregion
    }

    public EditCommunicationRequestValidator(
      ICommunicationRepository communicationRepository)
    {
      _communicationRepository = communicationRepository;

      RuleForEach(x => x.Operations)
       .CustomAsync(async (x, context, _) => await HandleInternalPropertyValidation(x, context));
    }
  }
}
