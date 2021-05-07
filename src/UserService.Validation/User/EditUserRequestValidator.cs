using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Validators;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Validation.User.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json;

namespace LT.DigitalOffice.UserService.Validation.User
{
    public class EditUserRequestValidator : AbstractValidator<JsonPatchDocument<EditUserRequest>>, IEditUserRequestValidator
    {
        private static Regex NameRegex = new("^[A-Z][a-z]+$|^[А-ЯЁ][а-яё]+$");

        private void HandleInternalPropertyValidation(Operation<EditUserRequest> requestedOperation, CustomContext context)
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
                Dictionary<Func<Operation<EditUserRequest>, bool>, string> predicates)
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
                    nameof(EditUserRequest.FirstName),
                    nameof(EditUserRequest.MiddleName),
                    nameof(EditUserRequest.LastName),
                    nameof(EditUserRequest.Status),
                    nameof(EditUserRequest.Rate),
                    nameof(EditUserRequest.AvatarImage)
                });

            AddСorrectOperations(nameof(EditUserRequest.FirstName), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditUserRequest.MiddleName), new List<OperationType> { OperationType.Replace, OperationType.Add, OperationType.Remove });
            AddСorrectOperations(nameof(EditUserRequest.LastName), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditUserRequest.Status), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditUserRequest.Rate), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditUserRequest.AvatarImage), new List<OperationType> { OperationType.Replace, OperationType.Add, OperationType.Remove });

            #endregion

            #region firstname

            AddFailureForPropertyIf(
                nameof(EditUserRequest.FirstName),
                x => x == OperationType.Replace,
                new Dictionary<Func<Operation<EditUserRequest>, bool>, string>
                {
                    { x => !string.IsNullOrEmpty(x.value.ToString()), "FirstName is too short" },
                    { x => x.value.ToString().Length < 32, "FirstName is too long" },
                    { x => NameRegex.IsMatch(x.value.ToString()), "FirstName has incorrect format" }
                });

            #endregion

            #region lastname

            AddFailureForPropertyIf(
                nameof(EditUserRequest.LastName),
                x => x == OperationType.Replace,
                new Dictionary<Func<Operation<EditUserRequest>, bool>, string>
                {
                    { x => !string.IsNullOrEmpty(x.value.ToString()), "LastName is too short" },
                    { x => x.value.ToString().Length < 100, "LastName is too long" },
                    { x => NameRegex.IsMatch(x.value.ToString()), "LastName has incorrect format" }
                });

            #endregion

            #region middlename

            AddFailureForPropertyIf(
                nameof(EditUserRequest.MiddleName),
                x => x == OperationType.Replace || x == OperationType.Add,
                new Dictionary<Func<Operation<EditUserRequest>, bool>, string>
                {
                    { x => !string.IsNullOrEmpty(x.value.ToString()), "MiddleName is too short" },
                    { x => x.value.ToString().Length < 32, "MiddleName is too long" },
                    { x => NameRegex.IsMatch(x.value.ToString()), "MiddleName has incorrect format" }
                });

            #endregion

            #region Status

            AddFailureForPropertyIf(
                nameof(EditUserRequest.Status),
                x => x == OperationType.Replace,
                new Dictionary<Func<Operation<EditUserRequest>, bool>, string>
                {
                    { x => Enum.TryParse(typeof(UserStatus), x.value?.ToString(), out _), "Incorrect user status"},
                });

            #endregion

            #region Rate

            AddFailureForPropertyIf(
                nameof(EditUserRequest.Rate),
                x => x == OperationType.Replace,
                new Dictionary<Func<Operation<EditUserRequest>, bool>, string>
                {
                    { x => double.TryParse(x.value?.ToString(), out _), "Incorrect rate format"},
                    { x =>
                        {
                            if (double.TryParse(x.value?.ToString(), out double rate))
                            {
                                return rate > 0;
                            }

                            return false;
                        },
                        "Rate must be greater than 0"
                    },
                    { x =>
                        {
                            if (double.TryParse(x.value?.ToString(), out double rate))
                            {
                                return rate < 3; // I am not sure
                            }

                            return false;
                        },
                        "Rate must be less than 3"
                    }
                });

            #endregion

            #region AvatarImage

            AddFailureForPropertyIf(
                nameof(EditUserRequest.AvatarImage),
                x => x == OperationType.Replace,
                new Dictionary<Func<Operation<EditUserRequest>, bool>, string>
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

            #endregion
        }

        public EditUserRequestValidator()
        {
            RuleForEach(x => x.Operations)
               .Custom(HandleInternalPropertyValidation);
        }
    }
}