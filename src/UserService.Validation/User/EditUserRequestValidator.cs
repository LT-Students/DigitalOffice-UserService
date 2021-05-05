using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Validation.User.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace LT.DigitalOffice.UserService.Validation.User
{
    public class EditUserRequestValidator : AbstractValidator<JsonPatchDocument<EditUserRequest>>, IEditUserRequestValidator
    {
        private static List<string> Paths => new List<string>
        {
            $"/{nameof(EditUserRequest.FirstName)}",
            $"/{nameof(EditUserRequest.MiddleName)}",
            $"/{nameof(EditUserRequest.LastName)}",
            $"/{nameof(EditUserRequest.Image)}",
            $"/{nameof(EditUserRequest.Status)}",
            $"/{nameof(EditUserRequest.Rate)}"
        };

        public EditUserRequestValidator()
        {
            RuleFor(x => x.Operations)
                .Must(x =>
                    x.Select(x => x.path)
                        .Distinct().Count() == x.Count
                )
                .WithMessage("You don't have to change the same field of Project multiple times.")
                .Must(x => x.Any())
                .WithMessage("You don't have changes.")
                .ForEach(y => y
                    .Must(x => Paths.Any(cur => string.Equals(cur, x.path, StringComparison.OrdinalIgnoreCase)))
                    .WithMessage(
                        $"Document contains invalid path. Only such paths are allowed: {Paths.Aggregate((x, y) => x + ", " + y)}")
                )
                .DependentRules(() =>
                {
                    When(o => o.Operations != null, () =>
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
                                else if (o.path.EndsWith(nameof(EditEducationRequest.IsActive), StringComparison.OrdinalIgnoreCase))
                                {
                                    return bool.TryParse(value, out _);
                                }
                                else if (o.path.EndsWith(nameof(EditEducationRequest.AdmissionAt), StringComparison.OrdinalIgnoreCase))
                                {
                                    return DateTime.TryParse(value, out _);
                                }
                                else if (o.path.EndsWith(nameof(EditEducationRequest.IssueAt), StringComparison.OrdinalIgnoreCase))
                                {
                                    return DateTime.TryParse(value, out _);
                                }

                                return true;
                            });
                    });

                    When(x => GetOperationByPath(x, FirstName) != null, () =>
                    {
                        RuleFor(x => x.Operations)
                            .UniqueOperationWithAllowedOp(FirstName, "replace");

                        RuleFor(x => (string) GetOperationByPath(x, FirstName).value)
                            .NotEmpty()
                            .MaximumLength(32).WithMessage("First name is too long.")
                            .MinimumLength(1).WithMessage("First name is too short.")
                            .Matches("^[A-Z][a-z]+$|^[А-ЯЁ][а-яё]+$").WithMessage("First name with error.");
                    });

                    When(x => GetOperationByPath(x, LastName) != null, () =>
                    {
                        RuleFor(x => x.Operations)
                            .UniqueOperationWithAllowedOp(LastName, "replace");

                        RuleFor(x => (string) GetOperationByPath(x, LastName).value)
                            .NotEmpty()
                            .MaximumLength(32).WithMessage("Last name is too long.")
                            .MinimumLength(1).WithMessage("Last name is too short.")
                            .Matches("^[A-Z][a-z]+$|^[А-ЯЁ][а-яё]+$").WithMessage("Last name with error.");
                    });

                    When(x => GetOperationByPath(x, MiddleName) != null, () =>
                    {
                        RuleFor(x => x.Operations)
                            .UniqueOperationWithAllowedOp(MiddleName, "add", "replace", "remove");

                        RuleFor(x => (string) GetOperationByPath(x, MiddleName).value)
                            .MaximumLength(32).WithMessage("Middle name is too long.")
                            .MinimumLength(1).WithMessage("Middle name is too short.")
                            .Matches("^[A-Z][a-z]+$|^[А-ЯЁ][а-яё]+$").WithMessage("Middle name with error.");
                    });

                    When(x => GetOperationByPath(x, Status) != null, () =>
                    {
                        RuleFor(x => x.Operations)
                            .UniqueOperationWithAllowedOp(Status, "add", "replace", "remove");

                        RuleFor(x => (UserStatus) GetOperationByPath(x, Status).value)
                            .IsInEnum().WithMessage("Wrong status value.");
                    });

                    When(x => GetOperationByPath(x, AvatarImage) != null, () =>
                    {
                        RuleFor(x => x.Operations)
                            .UniqueOperationWithAllowedOp(AvatarImage, "add", "replace", "remove");

                        RuleFor(x => (string) GetOperationByPath(x, AvatarImage).value)
                            .NotEmpty()
                            .NotNull()
                            .Must(x => Convert
                                .TryFromBase64String(x, new Span<byte>(new byte[x.Length]), out _));
                    });
                });
        }
    }
}