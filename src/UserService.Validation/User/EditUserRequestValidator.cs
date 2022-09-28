using FluentValidation;
using LT.DigitalOffice.Kernel.Validators;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Validation.User.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LT.DigitalOffice.UserService.Validation.User
{
  public class EditUserRequestValidator : BaseEditRequestValidator<EditUserRequest>, IEditUserRequestValidator
  {
    private static Regex NameRegex = new(@"\d");

    private void HandleInternalPropertyValidation(Operation<EditUserRequest> requestedOperation, ValidationContext<JsonPatchDocument<EditUserRequest>> context)
    {
      Context = context;
      RequestedOperation = requestedOperation;

      #region paths

      AddСorrectPaths(
        new List<string>
        {
          nameof(EditUserRequest.FirstName),
          nameof(EditUserRequest.MiddleName),
          nameof(EditUserRequest.LastName),
          nameof(EditUserRequest.Status),
          nameof(EditUserRequest.IsAdmin),
          nameof(EditUserRequest.GenderId),
          nameof(EditUserRequest.DateOfBirth),
          nameof(EditUserRequest.About),
          nameof(EditUserRequest.BusinessHoursFromUtc),
          nameof(EditUserRequest.BusinessHoursToUtc),
          nameof(EditUserRequest.Latitude),
          nameof(EditUserRequest.Longitude),
        });

      AddСorrectOperations(nameof(EditUserRequest.FirstName), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(EditUserRequest.MiddleName), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(EditUserRequest.LastName), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(EditUserRequest.Status), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(EditUserRequest.IsAdmin), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(EditUserRequest.GenderId), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(EditUserRequest.DateOfBirth), new List<OperationType> { OperationType.Replace }); 
      AddСorrectOperations(nameof(EditUserRequest.About), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(EditUserRequest.BusinessHoursFromUtc), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(EditUserRequest.BusinessHoursToUtc), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(EditUserRequest.Latitude), new List<OperationType> { OperationType.Replace });
      AddСorrectOperations(nameof(EditUserRequest.Longitude), new List<OperationType> { OperationType.Replace });
     
      #endregion

      #region firstname

      AddFailureForPropertyIf(
        nameof(EditUserRequest.FirstName),
        x => x == OperationType.Replace,
        new Dictionary<Func<Operation<EditUserRequest>, bool>, string>
        {
          { x => !string.IsNullOrEmpty(x.value?.ToString().Trim()), "First name must not be empty." },
          { x => !NameRegex.IsMatch(x.value.ToString()), "First name must not contain numbers." },
          { x => x.value.ToString().Trim().Length < 46, "First name is too long." },
        }, CascadeMode.Stop);

      #endregion

      #region lastname

      AddFailureForPropertyIf(
        nameof(EditUserRequest.LastName),
        x => x == OperationType.Replace,
        new Dictionary<Func<Operation<EditUserRequest>, bool>, string>
        {
          { x => !string.IsNullOrEmpty(x.value?.ToString().Trim()), "Last name must not be empty." },
          { x => !NameRegex.IsMatch(x.value.ToString()), "Last name must not contain numbers." },
          { x => x.value.ToString().Trim().Length < 46, "Last name is too long." },
        }, CascadeMode.Stop);

      #endregion

      #region middlename

      AddFailureForPropertyIf(
        nameof(EditUserRequest.MiddleName),
        x => x == OperationType.Replace,
        new Dictionary<Func<Operation<EditUserRequest>, bool>, string>
        {
          {
            x => string.IsNullOrEmpty(x.value?.ToString())? true :
            (x.value.ToString().Trim().Length < 46), "Middle name is too long."
          },
          {
            x => string.IsNullOrEmpty(x.value?.ToString())? true :
            (!NameRegex.IsMatch(x.value.ToString())), "Middle name must not contain numbers."
          },
        });

      #endregion

      #region Status

      AddFailureForPropertyIf(
        nameof(EditUserRequest.Status),
        x => x == OperationType.Replace,
        new Dictionary<Func<Operation<EditUserRequest>, bool>, string>
        {
          { x => Enum.TryParse(typeof(UserStatus), x.value?.ToString(), out _), "Incorrect user status."}
        });

      #endregion

      #region IsAdmin 

      AddFailureForPropertyIf(
       nameof(EditUserRequest.IsAdmin),
       x => x == OperationType.Replace,
       new Dictionary<Func<Operation<EditUserRequest>, bool>, string>
       {
          { x => bool.TryParse(x.value?.ToString(), out bool _), "Incorrect user is admin format" },
       });

      #endregion

      #region GenderId 

      AddFailureForPropertyIf(
        nameof(EditUserRequest.GenderId),
        x => x == OperationType.Replace,
        new Dictionary<Func<Operation<EditUserRequest>, bool>, string>
        {
          { x => string.IsNullOrEmpty(x.value?.ToString())? true :
            Guid.TryParse(x.value.ToString(), out Guid result),
            "Incorrect format of GenderId." },
        });

      #endregion

      #region DateOfBirth

      AddFailureForPropertyIf(
        nameof(EditUserRequest.DateOfBirth),
        x => x == OperationType.Replace,
        new Dictionary<Func<Operation<EditUserRequest>, bool>, string>
        {
          {
            x => string.IsNullOrEmpty(x.value?.ToString())? true :
              DateTime.TryParse(x.value.ToString(), out DateTime result),
            "Date of birth has incorrect format."
          },
        });

      #endregion

      #region About 

      AddFailureForPropertyIf(
        nameof(EditUserRequest.About),
        x => x == OperationType.Replace,
        new Dictionary<Func<Operation<EditUserRequest>, bool>, string>
        {
          {x => x.value?.ToString().Trim().Length < 151, "About is too long." },
        });

      #endregion

      #region BusinessHoursFromUtc

      AddFailureForPropertyIf(
      nameof(EditUserRequest.BusinessHoursFromUtc), 
      x => x == OperationType.Replace, 
      new Dictionary<Func<Operation<EditUserRequest>, bool>, string>
      {
        { x => string.IsNullOrEmpty(x.value?.ToString())? true :
          DateTime.TryParse(x.value.ToString(), out DateTime result),
        "Incorrect format of BusinessHoursFromUtc."
        }
      });

      #endregion

      #region BusinessHoursToUtc

      AddFailureForPropertyIf(
      nameof(EditUserRequest.BusinessHoursToUtc),
      x => x == OperationType.Replace,
      new Dictionary<Func<Operation<EditUserRequest>, bool>, string>
      {
        { x => string.IsNullOrEmpty(x.value?.ToString())? true :
          DateTime.TryParse(x.value.ToString(), out DateTime result),
         "Incorrect format of BusinessHoursToUtc."
        }
      });

      #endregion

      #region Latitude

      AddFailureForPropertyIf(
        nameof(EditUserRequest.Latitude),
        x => x == OperationType.Replace,
        new Dictionary<Func<Operation<EditUserRequest>, bool>, string>
        {
          { x => string.IsNullOrEmpty(x.value?.ToString())? true :
            double.TryParse(x.value.ToString(), out double _),
            "Incorrect format of Latitude." },
        });

      #endregion

      #region Longitude

      AddFailureForPropertyIf(
       nameof(EditUserRequest.Longitude),
       x => x == OperationType.Replace,
       new Dictionary<Func<Operation<EditUserRequest>, bool>, string>
       {
         { x => string.IsNullOrEmpty(x.value?.ToString())? true :
           double.TryParse(x.value.ToString(), out double _),
           "Incorrect format of Longitude." },
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