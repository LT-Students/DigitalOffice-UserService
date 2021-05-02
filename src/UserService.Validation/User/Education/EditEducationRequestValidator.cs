using FluentValidation;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using LT.DigitalOffice.UserService.Validation.User.Interfaces.Education;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Validation.User.Education
{
    public class EditEducationRequestValidator : AbstractValidator<JsonPatchDocument<EditEducationRequest>>, IEditEducationRequestValidator
    {
        private static List<string> Paths
            => new() { UniversityName, QualificationName, FormEducation, AdmissiomAt, IssueAt };

        public static string UniversityName => $"/{nameof(EditEducationRequest.UniversityName)}";
        public static string QualificationName => $"/{nameof(EditEducationRequest.QualificationName)}";
        public static string FormEducation => $"/{nameof(EditEducationRequest.FormEducation)}";
        public static string AdmissiomAt => $"/{nameof(EditEducationRequest.AdmissiomAt)}";
        public static string IssueAt => $"/{nameof(EditEducationRequest.IssueAt)}";

        Func<JsonPatchDocument<EditEducationRequest>, string, Operation> GetOperationByPath =>
            (x, path) => x.Operations.FirstOrDefault(x => x.path == path);

        public EditEducationRequestValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Operations)
                .Must(x =>
                    x.Select(x => x.path)
                        .Distinct().Count() == x.Count())
                .WithMessage("You don't have to change the same field of Education multiple times.")
                .Must(x => x.Any())
                .WithMessage("You don't have changes.")
                .ForEach(y => y
                    .Must(x => Paths.Any(cur => string.Equals(
                        cur,
                        x.path,
                        StringComparison.OrdinalIgnoreCase)))
                    .WithMessage(
                        $"Document contains invalid path. Only such paths are allowed: {Paths.Aggregate((x, y) => x + ", " + y)}")
                )
                .DependentRules(() =>
                {
                    When(x => GetOperationByPath(x, UniversityName) != null, () =>
                    {
                        RuleFor(x => x.Operations)
                            .UniqueOperationWithAllowedOp(UniversityName, "replace");

                        RuleFor(x => (string)GetOperationByPath(x, UniversityName).value)
                            .NotEmpty()
                            .MaximumLength(50)
                            .WithMessage("University name is too long");
                    });

                    When(x => GetOperationByPath(x, QualificationName) != null, () =>
                    {
                        RuleFor(x => x.Operations)
                            .UniqueOperationWithAllowedOp(QualificationName, "replace");

                        RuleFor(x => (string)GetOperationByPath(x, QualificationName).value)
                            .NotEmpty()
                            .MaximumLength(50)
                            .WithMessage("Qualification name is too long");
                    });

                    When(x => GetOperationByPath(x, FormEducation) != null, () =>
                    {
                        RuleFor(x => x.Operations)
                            .UniqueOperationWithAllowedOp(FormEducation, "replace");

                        RuleFor(x => GetOperationByPath(x, FormEducation))
                            .Must(e => e.value != null)
                            .Must(e => Enum.TryParse(e.value.ToString(), out FormEducation _))
                            .WithMessage("Wrong form education.");
                    });
                });
        }
    }
}
