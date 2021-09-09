using FluentValidation.Results;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.UserService.Business.Commands.Avatar.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Avatars;
using LT.DigitalOffice.UserService.Validation.Avatars.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace LT.DigitalOffice.UserService.Business.Commands.Avatar
{
  public class RemoveAvatarsCommand : IRemoveAvatarsCommand
  {
    private readonly IAvatarRepository _avatarRepository;
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRequestClient<IRemoveImagesRequest> _rcRemoveImages;
    private readonly IRemoveAvatarsRequestValidator _removeRequestValidator;
    private readonly IAccessValidator _accessValidator;
    private readonly ILogger<RemoveAvatarsCommand> _logger;

    private bool RemoveImages(List<Guid> imageIds, Guid userId, List<string> errors)
    {
      string errorMessage = "Can't remove images. Please try again later.";
      string logMessage = "Errors while removing images with ids: {Ids}.";

      try
      {
        Response<IOperationResult<bool>> removeResponse = _rcRemoveImages.GetResponse<IOperationResult<bool>>(
          IRemoveImagesRequest.CreateObj(imageIds, ImageSource.User))
          .Result;

        if (removeResponse.Message.IsSuccess)
        {
          return removeResponse.Message.Body;
        }

        string warningMessage = logMessage + "Errors: {Errors}";

        _logger.LogWarning(
          warningMessage,
          string.Join(", ", imageIds),
          string.Join('\n', removeResponse.Message.Errors));

        errors.Add(errorMessage);
      }
      catch (Exception e)
      {
        _logger.LogError(e, logMessage, string.Join(", ", imageIds));

        errors.Add(errorMessage);
      }

      return false;
    }

    private bool RemoveImagesFromDb(List<Guid> imageIds, Guid userId)
    {
      DbUser dbUser = _userRepository.Get(userId);

      if (dbUser.AvatarFileId != null && imageIds.Contains(dbUser.AvatarFileId.Value))
      {
        _userRepository.UpdateAvatar(userId, null);
      }

      return _avatarRepository.Remove(imageIds);
    }

    public RemoveAvatarsCommand(
      IAvatarRepository avatarRepository,
      IUserRepository userRepository,
      IHttpContextAccessor httpContextAccessor,
      IRequestClient<IRemoveImagesRequest> rcRemoveImages,
      IRemoveAvatarsRequestValidator removeRequestValidator,
      IAccessValidator accessValidator,
      ILogger<RemoveAvatarsCommand> logger)
    {
      _avatarRepository = avatarRepository;
      _userRepository = userRepository;
      _httpContextAccessor = httpContextAccessor;
      _rcRemoveImages = rcRemoveImages;
      _removeRequestValidator = removeRequestValidator;
      _accessValidator = accessValidator;
      _logger = logger;
    }

    public OperationResultResponse<bool> Execute(RemoveAvatarsRequest request)
    {
      OperationResultResponse<bool> response = new();
      List<string> errors = new();

      Guid senderId = _httpContextAccessor.HttpContext.GetUserId();

      if (!_accessValidator.HasRights(Rights.AddEditRemoveUsers)
        && senderId != request.UserId)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        response.Status = OperationResultStatusType.Failed;
        response.Errors.Add("Not enough rights.");

        return response;
      }

      if (_removeRequestValidator.ValidateCustom(request, out errors) == false)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        response.Status = OperationResultStatusType.Failed;
        response.Errors.AddRange(errors);

        return response;
      }

      response.Body = RemoveImages(request.AvatarIds, request.UserId, response.Errors);

      if (response.Body)
      {
        RemoveImagesFromDb(request.AvatarIds, request.UserId);
      }

      response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;
      return response;
    }
  }
}
