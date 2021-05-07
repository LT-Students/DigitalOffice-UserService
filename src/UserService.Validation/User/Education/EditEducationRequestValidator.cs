using FluentValidation;
using FluentValidation.Validators;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using LT.DigitalOffice.UserService.Validation.User.Interfaces.Education;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LT.DigitalOffice.UserService.Validation.User.Education
{
    public class EditEducationRequestValidator : AbstractValidator<JsonPatchDocument<EditEducationRequest>>, IEditEducationRequestValidator
    {
        private void HandleInternalPropertyValidation(Operation<EditEducationRequest> requestedOperation, CustomContext context)
        {
            #region local functions

            void AddСorrectPaths(
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

            AddСorrectPaths(nameof(EditEducationRequest.UniversityName), new List<OperationType> { OperationType.Replace });
            AddСorrectPaths(nameof(EditEducationRequest.QualificationName), new List<OperationType> { OperationType.Replace });
            AddСorrectPaths(nameof(EditEducationRequest.FormEducation), new List<OperationType> { OperationType.Replace });
            AddСorrectPaths(nameof(EditEducationRequest.AdmissionAt), new List<OperationType> { OperationType.Replace });
            AddСorrectPaths(nameof(EditEducationRequest.IssueAt), new List<OperationType> { OperationType.Replace, OperationType.Add, OperationType.Remove });
            AddСorrectPaths(nameof(EditEducationRequest.IsActive), new List<OperationType> { OperationType.Replace });

            #endregion
        }

        public EditEducationRequestValidator()
        {
            RuleForEach(x => x.Operations)
               .Custom(HandleInternalPropertyValidation);
        }
        //private static List<string> Paths
        //    => new()
        //    {
        //        $"/{nameof(EditEducationRequest.UniversityName)}",
        //        $"/{nameof(EditEducationRequest.QualificationName)}",
        //        $"/{nameof(EditEducationRequest.FormEducation)}",
        //        $"/{nameof(EditEducationRequest.AdmissionAt)}",
        //        $"/{nameof(EditEducationRequest.IssueAt)}",
        //        $"/{nameof(EditEducationRequest.IsActive)}"
        //    };

        //public EditEducationRequestValidator()
        //{
        //    CascadeMode = CascadeMode.Stop;

        //    RuleFor(x => x.Operations)
        //        .Must(x =>
        //            x.Select(x => x.path)
        //                .Distinct().Count() == x.Count())
        //        .WithMessage("You don't have to change the same field of Education multiple times.")
        //        .Must(x => x.Any())
        //        .WithMessage("You don't have changes.")
        //        .ForEach(y => y
        //            .Must(x => Paths.Any(cur => string.Equals(
        //                cur,
        //                x.path,
        //                StringComparison.OrdinalIgnoreCase)))
        //            .WithMessage(
        //                $"Document contains invalid path. Only such paths are allowed: {Paths.Aggregate((x, y) => x + ", " + y)}")
        //        )
        //        .DependentRules(() =>
        //        {
        //            When(x => x.Operations != null, () =>
        //            {
        //                RuleForEach(x => x.Operations)
        //                    .Must(o =>
        //                        {
        //                            string value = o.value?.ToString();

        //                            if (string.IsNullOrEmpty(value))
        //                            {
        //                                return false;
        //                            }

        //                            if (o.path.EndsWith(nameof(EditEducationRequest.UniversityName), StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                return value.Length < 100;
        //                            }
        //                            else if (o.path.EndsWith(nameof(EditEducationRequest.QualificationName), StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                return value.Length < 100;
        //                            }
        //                            else if (o.path.EndsWith(nameof(EditEducationRequest.FormEducation), StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                return Enum.TryParse(typeof(FormEducation), value, out _);
        //                            }
        //                            else if (o.path.EndsWith(nameof(EditEducationRequest.IsActive), StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                return bool.TryParse(value, out _);
        //                            }
        //                            else if (o.path.EndsWith(nameof(EditEducationRequest.AdmissionAt), StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                return DateTime.TryParse(value, out _);
        //                            }
        //                            else if (o.path.EndsWith(nameof(EditEducationRequest.IssueAt), StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                return DateTime.TryParse(value, out _);
        //                            }

        //                            return false;
        //                        });
        //            });

        //        });
        //}
    }
}
