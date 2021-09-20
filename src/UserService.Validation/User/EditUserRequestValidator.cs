using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Validators;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json;

namespace LT.DigitalOffice.UserService.Validation.User
{
    public class EditUserRequestValidator : AbstractValidator<JsonPatchDocument<EditUserRequest>>
    {
        private static Regex NameRegex = new(@"\d");
        private readonly IImageRepository _imageRepository;

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
                    nameof(EditUserRequest.City),
                    nameof(EditUserRequest.Gender),
                    nameof(EditUserRequest.DateOfBirth),
                    nameof(EditUserRequest.StartWorkingAt),
                    nameof(EditUserRequest.About),
                    nameof(EditUserRequest.AvatarFileId),
                    nameof(EditUserRequest.IsActive),
                    nameof(EditUserRequest.DepartmentId),
                    nameof(EditUserRequest.RoleId),
                    nameof(EditUserRequest.PositionId),
                    nameof(EditUserRequest.OfficeId),
                });

            AddСorrectOperations(nameof(EditUserRequest.FirstName), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditUserRequest.MiddleName), new List<OperationType> { OperationType.Replace, OperationType.Add, OperationType.Remove });
            AddСorrectOperations(nameof(EditUserRequest.LastName), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditUserRequest.Status), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditUserRequest.Rate), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditUserRequest.Gender), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditUserRequest.City), new List<OperationType> { OperationType.Replace, OperationType.Add, OperationType.Remove });
            AddСorrectOperations(nameof(EditUserRequest.DateOfBirth), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditUserRequest.StartWorkingAt), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditUserRequest.AvatarFileId), new List<OperationType> { OperationType.Replace, OperationType.Add, OperationType.Remove });
            AddСorrectOperations(nameof(EditUserRequest.IsActive), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditUserRequest.DepartmentId), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditUserRequest.PositionId), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditUserRequest.RoleId), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditUserRequest.OfficeId), new List<OperationType> { OperationType.Replace });
            AddСorrectOperations(nameof(EditUserRequest.About), new List<OperationType> { OperationType.Replace, OperationType.Add, OperationType.Remove });

            #endregion

            #region firstname

            AddFailureForPropertyIf(
                nameof(EditUserRequest.FirstName),
                x => x == OperationType.Replace,
                new Dictionary<Func<Operation<EditUserRequest>, bool>, string>
                {
                    { x => !NameRegex.IsMatch(x.value.ToString()), "First name must not contain numbers" },
                    { x => !string.IsNullOrEmpty(x.value.ToString()), "First name is empty" },
                    { x => x.value.ToString().Length < 32, "First name is too long" }
                });

            #endregion

            #region lastname

            AddFailureForPropertyIf(
                nameof(EditUserRequest.LastName),
                x => x == OperationType.Replace,
                new Dictionary<Func<Operation<EditUserRequest>, bool>, string>
                {
                    { x => !NameRegex.IsMatch(x.value.ToString()), "Last name must not contain numbers" },
                    { x => !string.IsNullOrEmpty(x.value.ToString()), "Last name is empty" },
                    { x => x.value.ToString().Length < 100, "Last name is too long" }
                });

            #endregion

            #region middlename

            AddFailureForPropertyIf(
                nameof(EditUserRequest.MiddleName),
                x => x == OperationType.Replace || x == OperationType.Add,
                new Dictionary<Func<Operation<EditUserRequest>, bool>, string>
                {
                    { x => !NameRegex.IsMatch(x.value.ToString()), "Middle name must not contain numbers" },
                    { x => !string.IsNullOrEmpty(x.value.ToString()), "Middle name is empty" },
                    { x => x.value.ToString().Length < 32, "Middle name is too long" }
                });

            #endregion

            #region City

            AddFailureForPropertyIf(
                nameof(EditUserRequest.City),
                x => x == OperationType.Replace || x == OperationType.Add,
                new Dictionary<Func<Operation<EditUserRequest>, bool>, string>
                {
                    { x => !string.IsNullOrEmpty(x.value.ToString()), "City name is empty" },
                    { x => x.value.ToString().Length < 32, "City name is too long" }
                });

            #endregion

            #region Gender

            AddFailureForPropertyIf(
                nameof(EditUserRequest.Gender),
                x => x == OperationType.Replace,
                new Dictionary<Func<Operation<EditUserRequest>, bool>, string>
                {
                    { x => Enum.TryParse(typeof(UserGender), x.value?.ToString(), out _), "Incorrect user gender"},
                });

            #endregion

            #region Status

            AddFailureForPropertyIf(
                nameof(EditUserRequest.Status),
                x => x == OperationType.Replace,
                new Dictionary<Func<Operation<EditUserRequest>, bool>, string>
                {
                    { x => Enum.TryParse(typeof(UserStatus), x.value?.ToString(), out _), "Incorrect user status"}
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
                                return rate <= 1;
                            }

                            return false;
                        },
                        "Rate must be less than 1"
                    }
                });

            #endregion

            #region DateOfBirth

            AddFailureForPropertyIf(
                nameof(EditUserRequest.DateOfBirth),
                x => x == OperationType.Replace,
                new Dictionary<Func<Operation<EditUserRequest>, bool>, string>
                {
                    { x =>
                        {
                            if(!string.IsNullOrEmpty(x.value?.ToString()))
                            {
                                return DateTime.TryParse(x.value.ToString(), out DateTime result);
                            }
                            return true;
                        },
                        "Date of birth has incorrect format"
                    }
                });

            #endregion

            #region StartWorkingAt

            AddFailureForPropertyIf(
                nameof(EditUserRequest.StartWorkingAt),
                x => x == OperationType.Replace,
                new Dictionary<Func<Operation<EditUserRequest>, bool>, string>
                {
                     { x =>
                        {
                            if(!string.IsNullOrEmpty(x.value?.ToString()))
                            {
                                return DateTime.TryParse(x.value.ToString(), out DateTime result);
                            }
                            return true;
                        },
                        "Start working at has incorrect format"
                     }
                });

            #endregion

            #region AvatarId

            AddFailureForPropertyIf(
                nameof(EditUserRequest.AvatarFileId),
                x => x == OperationType.Replace,
                new Dictionary<Func<Operation<EditUserRequest>, bool>, string>
                {
                    { x =>
                        {
                            try
                            {
                                Guid avatarId = Guid.Parse(x.value?.ToString());

                                if (_imageRepository.Get(new List<Guid> {avatarId}).Any())
                                {
                                    return true;
                                }
                            }
                            catch
                            {
                            }

                            return false;
                        },
                        "Incorrect ImageId."
                    }
            });

      #endregion

      #region IsActive

      AddFailureForPropertyIf(
                nameof(EditUserRequest.IsActive),
                x => x == OperationType.Replace,
                new Dictionary<Func<Operation<EditUserRequest>, bool>, string>
                {
                    { x => bool.TryParse(x.value?.ToString(), out _), "Incorrect user is active format" }
                });

            #endregion

            #region DepartmentId

            AddFailureForPropertyIf(
                nameof(EditUserRequest.DepartmentId),
                x => x == OperationType.Replace,
                new()
                {
                    { x => Guid.TryParse(x.value.ToString(), out Guid result), "Department id has incorrect format" }
                });

            #endregion

            #region PositionId

            AddFailureForPropertyIf(
                nameof(EditUserRequest.PositionId),
                x => x == OperationType.Replace,
                new()
                {
                    { x => Guid.TryParse(x.value.ToString(), out Guid result), "Position id has incorrect format" }
                });

            #endregion

            #region RoleId

            AddFailureForPropertyIf(
                nameof(EditUserRequest.RoleId),
                x => x == OperationType.Replace,
                new()
                {
                    { x => Guid.TryParse(x.value.ToString(), out Guid result), "Role id has incorrect format" }
                });

            #endregion

            #region OfficeId

            AddFailureForPropertyIf(
                nameof(EditUserRequest.OfficeId),
                x => x == OperationType.Replace,
                new()
                {
                    { x => Guid.TryParse(x.value.ToString(), out Guid result), "Office id has incorrect format" }
                });

            #endregion

            #region About

            AddFailureForPropertyIf(
                nameof(EditUserRequest.About),
                x => x == OperationType.Replace || x == OperationType.Add,
                new()
                {
                    { x => !string.IsNullOrEmpty(x.value.ToString()), "About must not be empty." },
                });

            #endregion
        }

        public EditUserRequestValidator(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;

            RuleForEach(x => x.Operations)
               .Custom(HandleInternalPropertyValidation);
        }
    }
}