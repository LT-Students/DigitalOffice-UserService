using FluentValidation;
using FluentValidation.Validators;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using LT.DigitalOffice.UserService.Validation.Education.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Validation.Education
{
    public class EditEducationRequestValidator : AbstractValidator<JsonPatchDocument<EditEducationRequest>>, IEditEducationRequestValidator
    {
        private void HandleInternalPropertyValidation(Operation<EditEducationRequest> requestedOperation, CustomContext context)
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
                Dictionary<Func<Operation<EditEducationRequest>, bool>, string> predicates)
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
                    nameof(EditEducationRequest.UniversityName),
                    nameof(EditEducationRequest.QualificationName),
                    nameof(EditEducationRequest.FormEducation),
                    nameof(EditEducationRequest.AdmissionAt),
                    nameof(EditEducationRequest.IssueAt),
                    nameof(EditEducationRequest.IsActive),
                });

            AddСorrectOperations(nameof(EditEducationRequest.UniversityName), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditEducationRequest.QualificationName), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditEducationRequest.FormEducation), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditEducationRequest.AdmissionAt), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditEducationRequest.IssueAt), new List<OperationType> { OperationType.Replace, OperationType.Add, OperationType.Remove });
            AddСorrectOperations(nameof(EditEducationRequest.IsActive), new List<OperationType> { OperationType.Replace });

            #endregion

            AddFailureForPropertyIf(
                nameof(EditEducationRequest.UniversityName),
                o => o == OperationType.Replace,
                new Dictionary<Func<Operation<EditEducationRequest>, bool>, string>
                {
                    { x => !string.IsNullOrEmpty(x.value?.ToString()), "UniversityName is too short."},
                    { x => x.value.ToString().Length < 100, "UniversityName is too long."}
                });

            AddFailureForPropertyIf(
                nameof(EditEducationRequest.QualificationName),
                o => o == OperationType.Replace,
                new Dictionary<Func<Operation<EditEducationRequest>, bool>, string>
                {
                    { x => !string.IsNullOrEmpty(x.value?.ToString()), "QualificationName is too short."},
                    { x => x.value.ToString().Length < 100, "QualificationName is too long."}
                });

            AddFailureForPropertyIf(
                nameof(EditEducationRequest.FormEducation),
                o => o == OperationType.Replace,
                new Dictionary<Func<Operation<EditEducationRequest>, bool>, string>
                {
                    { x => Enum.TryParse(typeof(FormEducation), x.value?.ToString(), out _), "Incorrect format FormEducation"}
                });

            AddFailureForPropertyIf(
                nameof(EditEducationRequest.IssueAt),
                o => o == OperationType.Replace || o == OperationType.Add,
                new Dictionary<Func<Operation<EditEducationRequest>, bool>, string>
                {
                    { x => DateTime.TryParse(x.value?.ToString(), out _), "Incorrect format IssueAt"}
                });

            AddFailureForPropertyIf(
                nameof(EditEducationRequest.AdmissionAt),
                o => o == OperationType.Replace,
                new Dictionary<Func<Operation<EditEducationRequest>, bool>, string>
                {
                    { x => DateTime.TryParse(x.value?.ToString(), out _), "Incorrect format AdmissionAt"}
                });

            AddFailureForPropertyIf(
                nameof(EditEducationRequest.IsActive),
                o => o == OperationType.Replace,
                new Dictionary<Func<Operation<EditEducationRequest>, bool>, string>
                {
                    { x => bool.TryParse(x.value?.ToString(), out _), "Incorrect format IsActive"}
                });
        }

        public EditEducationRequestValidator()
        {
            RuleForEach(x => x.Operations)
               .Custom(HandleInternalPropertyValidation);
        }
    }
}
