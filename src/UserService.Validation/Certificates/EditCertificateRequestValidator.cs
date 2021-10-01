using FluentValidation;
using FluentValidation.Validators;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Certificates;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Validation.Certificates
{
    public class EditCertificateRequestValidator : AbstractValidator<JsonPatchDocument<EditCertificateRequest>>
    {
        private void HandleInternalPropertyValidation(Operation<EditCertificateRequest> requestedOperation, CustomContext context)
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
                Dictionary<Func<Operation<EditCertificateRequest>, bool>, string> predicates)
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
                    nameof(EditCertificateRequest.Name),
                    nameof(EditCertificateRequest.ReceivedAt),
                    nameof(EditCertificateRequest.SchoolName),
                    nameof(EditCertificateRequest.Image),
                    nameof(EditCertificateRequest.EducationType),
                    nameof(EditCertificateRequest.IsActive)
                });

            AddСorrectOperations(nameof(EditCertificateRequest.Name), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditCertificateRequest.ReceivedAt), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditCertificateRequest.SchoolName), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditCertificateRequest.Image), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditCertificateRequest.EducationType), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditCertificateRequest.IsActive), new List<OperationType> { OperationType.Replace });

            #endregion

            #region conditions

            AddFailureForPropertyIf(
                nameof(EditCertificateRequest.Name),
                o => o == OperationType.Replace,
                new Dictionary<Func<Operation<EditCertificateRequest>, bool>, string>
                {
                    { x => !string.IsNullOrEmpty(x.value?.ToString()), "Name is too short."},
                    { x => x.value.ToString().Length < 100, "Name is too long."}
                });

            AddFailureForPropertyIf(
                nameof(EditCertificateRequest.SchoolName),
                o => o == OperationType.Replace,
                new Dictionary<Func<Operation<EditCertificateRequest>, bool>, string>
                {
                    { x => !string.IsNullOrEmpty(x.value?.ToString()), "School name is too short."},
                    { x => x.value.ToString().Length < 100, "School name is too long."}
                });

            AddFailureForPropertyIf(
                nameof(EditCertificateRequest.EducationType),
                o => o == OperationType.Replace,
                new Dictionary<Func<Operation<EditCertificateRequest>, bool>, string>
                {
                    { x => Enum.TryParse(typeof(EducationType), x.value?.ToString(), out _), "Incorrect format EducationType"}
                });

            AddFailureForPropertyIf(
                nameof(EditCertificateRequest.ReceivedAt),
                o => o == OperationType.Replace,
                new Dictionary<Func<Operation<EditCertificateRequest>, bool>, string>
                {
                    { x => DateTime.TryParse(x.value?.ToString(), out _), "Incorrect format ReceivedAt"}
                });

            AddFailureForPropertyIf(
                nameof(EditCertificateRequest.Image),
                o => o == OperationType.Replace,
                new Dictionary<Func<Operation<EditCertificateRequest>, bool>, string>
                {
                    { x =>
                        {
                            try
                            {
                                _ = JsonConvert.DeserializeObject<AddImageRequest>(x.value?.ToString());
                                return true;
                            }
                            catch
                            {
                                return false;
                            }
                        },
                        "Incorrect Image format"
                    }
                });

            AddFailureForPropertyIf(
                nameof(EditCertificateRequest.IsActive),
                o => o == OperationType.Replace,
                new Dictionary<Func<Operation<EditCertificateRequest>, bool>, string>
                {
                    { x => bool.TryParse(x.value?.ToString(), out _), "Incorrect format IsActive"}
                });

            #endregion
        }

        public EditCertificateRequestValidator()
        {
            RuleForEach(x => x.Operations)
                .Custom(HandleInternalPropertyValidation);
        }
    }
}
