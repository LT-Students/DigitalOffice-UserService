using FluentValidation;
using FluentValidation.Validators;
using LT.DigitalOffice.Kernel.Validators;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Achievement;
using LT.DigitalOffice.UserService.Validation.Achievement.Interfaces;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Validation.Achievement
{
  public class EditAchievementRequestValidator : BaseEditRequestValidator<EditAchievementRequest>, IEditAchievementRequestValidator
  {
    private List<string> AllowedExtensions = new()
    { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tga" };

    private void HandleInternalPropertyValidation(Operation<EditAchievementRequest> requestedOperation, CustomContext context)
    {
      Context = context;
      RequestedOperation = requestedOperation;

      #region Paths

      AddСorrectPaths(
        new()
        {
          nameof(EditAchievementRequest.Name),
          nameof(EditAchievementRequest.Description),
          nameof(EditAchievementRequest.Image),
        });

      AddСorrectOperations(nameof(EditAchievementRequest.Name), new() { OperationType.Replace });
      AddСorrectOperations(nameof(EditAchievementRequest.Description), new() { OperationType.Replace });
      AddСorrectOperations(nameof(EditAchievementRequest.Image), new() { OperationType.Replace });

      #endregion

      #region Name

      AddFailureForPropertyIf(
        nameof(EditAchievementRequest.Name),
        x => x == OperationType.Replace,
        new()
        {
          { x => !string.IsNullOrEmpty(x.value.ToString()), "Name cannot be empty." },
          { x => x.value.ToString().Length < 100, "Name is too long." },
        });

      #endregion

      #region Description

      AddFailureForPropertyIf(
        nameof(EditAchievementRequest.Description),
        x => x == OperationType.Replace,
        new()
        {
          { x => !string.IsNullOrEmpty(x.value.ToString()), "Description cannot be empty." },
          { x => x.value.ToString().Length < 1000, "Description is too long." },
        });

      #endregion

      #region Image

      AddFailureForPropertyIf(
        nameof(EditAchievementRequest.Image),
        x => x == OperationType.Replace,
        new()
        {
          {
            x =>
            {
              try
              {
                ImageConsist image = JsonConvert.DeserializeObject<ImageConsist>(x.value?.ToString());

                Span<byte> byteString = new Span<byte>(new byte[image.Content.Length]);

                if (!String.IsNullOrEmpty(image.Content) &&
                  Convert.TryFromBase64String(image.Content, byteString, out _) &&
                  AllowedExtensions.Contains(image.Extension))
                {
                  return true;
                }
              }
              catch
              {
              }
              return false;
            },
            "Incorrect Image format"
          }
        });

      #endregion
    }

    public EditAchievementRequestValidator()
    {
      RuleForEach(x => x.Operations)
        .Custom(HandleInternalPropertyValidation);
    }
  }
}
