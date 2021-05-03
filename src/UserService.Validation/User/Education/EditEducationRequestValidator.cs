using FluentValidation;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using LT.DigitalOffice.UserService.Validation.User.Interfaces.Education;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Validation.User.Education
{
    public class EditEducationRequestValidator : AbstractValidator<JsonPatchDocument<EditEducationRequest>>, IEditEducationRequestValidator
    {
        private static List<string> Paths
            => new()
            {
                $"/{nameof(EditEducationRequest.UniversityName)}",
                $"/{nameof(EditEducationRequest.QualificationName)}",
                $"/{nameof(EditEducationRequest.FormEducation)}",
                $"/{nameof(EditEducationRequest.AdmissionAt)}",
                $"/{nameof(EditEducationRequest.IssueAt)}"
            };

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
                    When(x => x.Operations != null, () =>
                    {
                        RuleForEach(x => x.Operations)
                            .Must(o =>
                                {
                                    string value = o.value?.ToString();

                                    if (string.IsNullOrEmpty(value))
                                    {
                                        return false;
                                    }

                                    if (o.path.EndsWith(nameof(EditEducationRequest.UniversityName), StringComparison.OrdinalIgnoreCase))
                                    {
                                        return value.Length < 100;
                                    }
                                    else if (o.path.EndsWith(nameof(EditEducationRequest.QualificationName), StringComparison.OrdinalIgnoreCase))
                                    {
                                        return value.Length < 100;
                                    }
                                    else if (o.path.EndsWith(nameof(EditEducationRequest.FormEducation), StringComparison.OrdinalIgnoreCase))
                                    {
                                        return Enum.TryParse(typeof(FormEducation), value, out _);
                                    }

                                    return true;
                                });
                    });

                });
        }
    }
}
