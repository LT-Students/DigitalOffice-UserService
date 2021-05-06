using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation;
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
            $"/{nameof(EditUserRequest.AvatarImage)}",
            $"/{nameof(EditUserRequest.Status)}",
            $"/{nameof(EditUserRequest.Rate)}"
        };

        private static Regex NameRegex = new("^[A-Z][a-z]+$|^[А-ЯЁ][а-яё]+$");

        private bool ValidateFirstName(string value)
        {
            return value.Length < 32 && NameRegex.Match(value).Success;
        }

        private bool ValidateLastName(string value)
        {
            return value.Length < 100 && NameRegex.Match(value).Success;
        }

        private bool ValidateMiddleName(string value)
        {
            return value.Length < 32 && NameRegex.Match(value).Success;
        }

        private bool ValidateRate(string value)
        {
            return double.TryParse(value, out _);
        }

        private bool ValidateAvatarImage(object value)
        {
            return value is AddImageRequest;
        }

        private bool ValidateStatus(string value)
        {
            return Enum.TryParse(typeof(UserStatus), value, out _);
        }

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

                                if (o.OperationType == OperationType.Replace)
                                {
                                    if (o.path.EndsWith(nameof(EditUserRequest.FirstName), StringComparison.OrdinalIgnoreCase))
                                    {
                                        return ValidateFirstName(value);
                                    }
                                    else if (o.path.EndsWith(nameof(EditUserRequest.LastName), StringComparison.OrdinalIgnoreCase))
                                    {
                                        return ValidateLastName(value);
                                    }
                                    else if (o.path.EndsWith(nameof(EditUserRequest.MiddleName), StringComparison.OrdinalIgnoreCase))
                                    {
                                        return ValidateMiddleName(value);
                                    }
                                    else if (o.path.EndsWith(nameof(EditUserRequest.Rate), StringComparison.OrdinalIgnoreCase))
                                    {
                                        return ValidateRate(value);
                                    }
                                    else if (o.path.EndsWith(nameof(EditUserRequest.Status), StringComparison.OrdinalIgnoreCase))
                                    {
                                        return ValidateStatus(value);
                                    }
                                    else if (o.path.EndsWith(nameof(EditUserRequest.AvatarImage), StringComparison.OrdinalIgnoreCase))
                                    {
                                        return ValidateAvatarImage(o.value);
                                    }
                                }
                                else if (o.OperationType == OperationType.Remove)
                                {
                                    if (o.path.EndsWith(nameof(EditUserRequest.AvatarImage), StringComparison.OrdinalIgnoreCase))
                                    {
                                        return ValidateAvatarImage(o.value);
                                    }
                                }
                                else if (o.OperationType == OperationType.Add)
                                {
                                    if (o.path.EndsWith(nameof(EditUserRequest.MiddleName), StringComparison.OrdinalIgnoreCase))
                                    {
                                        return ValidateMiddleName(value);
                                    }
                                    if (o.path.EndsWith(nameof(EditUserRequest.AvatarImage), StringComparison.OrdinalIgnoreCase))
                                    {
                                        return ValidateAvatarImage(o.value);
                                    }
                                }

                                return false;
                            });
                    });
                });
        }
    }
}