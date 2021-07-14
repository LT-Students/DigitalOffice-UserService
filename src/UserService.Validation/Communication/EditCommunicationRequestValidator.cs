using FluentValidation;
using FluentValidation.Validators;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Communication;
using LT.DigitalOffice.UserService.Validation.Communication.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Validation.Communication
{
    public class EditCommunicationRequestValidator 
        : AbstractValidator<JsonPatchDocument<EditCommunicationRequest>>, IEditCommunicationRequestValidator
    {
        private void HandleInternalPropertyValidation(Operation<EditCommunicationRequest> requestedOperation, CustomContext context)
        {
            #region local functions

            void AddСorrectPaths(List<string> paths)
            {
                if (paths.FirstOrDefault(p => p.EndsWith(requestedOperation.path[1..], StringComparison.OrdinalIgnoreCase)) == null)
                {
                    context.AddFailure(requestedOperation.path, $"This path {requestedOperation.path} is not available");
                }
            }

            void AddСorrectOperations(
                string propertyName,
                List<OperationType> types)
            {
                if (requestedOperation.path.EndsWith(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !types.Contains(requestedOperation.OperationType))
                {
                    context.AddFailure(propertyName, $"This operation {requestedOperation.OperationType} is prohibited for {propertyName}");
                }
            }

            void AddFailureForPropertyIf(
                string propertyName,
                Func<OperationType, bool> type,
                Dictionary<Func<Operation<EditCommunicationRequest>, bool>, string> predicates)
            {
                if (!requestedOperation.path.EndsWith(propertyName, StringComparison.OrdinalIgnoreCase)
                    || !type(requestedOperation.OperationType))
                {
                    return;
                }

                foreach (var validateDelegate in predicates)
                {
                    if (!validateDelegate.Key(requestedOperation))
                    {
                        context.AddFailure(propertyName, validateDelegate.Value);
                    }
                }
            }

            #endregion

            #region paths

            AddСorrectPaths(
                new List<string>
                {
                    nameof(EditCommunicationRequest.Type),
                    nameof(EditCommunicationRequest.Value)
                });

            AddСorrectOperations(nameof(EditCommunicationRequest.Type), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditCommunicationRequest.Value), new List<OperationType> { OperationType.Replace });

            #endregion

            #region Value

            AddFailureForPropertyIf(
                nameof(EditCommunicationRequest.Value),
                x => x == OperationType.Replace,
                new()
                {
                    { x => !string.IsNullOrEmpty(x.value.ToString()), "Value must not be empty." }
                });

            #endregion

            #region Types

            AddFailureForPropertyIf(
                nameof(EditCommunicationRequest.Type),
                x => x == OperationType.Replace,
                new()
                {
                    { x => Enum.TryParse(typeof(CommunicationType), x.value.ToString(), out _), "Incorrect format of communication type." }
                });

            #endregion
        }

        public EditCommunicationRequestValidator()
        {
            RuleForEach(x => x.Operations)
               .Custom(HandleInternalPropertyValidation);
        }
    }
}
