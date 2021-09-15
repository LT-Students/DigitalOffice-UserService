using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.UserService.Business.Commands.Image.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Images;
using LT.DigitalOffice.UserService.Validation.Image.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace LT.DigitalOffice.UserService.Business.Commands.Image
{
  public class RemoveImagesCommand : IRemoveImagesCommand
  {
    private readonly IImageRepository _imageRepository;
    private readonly ICertificateRepository _certificateRepository;
    private readonly IEducationRepository _educationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRequestClient<IRemoveImagesRequest> _rcRemoveImages;
    private readonly IRemoveImagesRequestValidator _removeRequestValidator;
    private readonly IAccessValidator _accessValidator;
    private readonly ILogger<RemoveImagesCommand> _logger;

    private bool RemoveImages(List<Guid> imagesIds, Guid entityId, List<string> errors)
    {
      string errorMessage = "Can't remove images. Please try again later.";
      string logMessage = "Errors while removing images with ids: {Ids}.";

      try
      {
        Response<IOperationResult<bool>> removeResponse = _rcRemoveImages.GetResponse<IOperationResult<bool>>(
            IRemoveImagesRequest.CreateObj(imagesIds, ImageSource.User))
          .Result;

        if (removeResponse.Message.IsSuccess)
        {
          return removeResponse.Message.Body;
        }

        _logger.LogWarning(
          logMessage,
          string.Join(", ", imagesIds),
          string.Join('\n', removeResponse.Message.Errors));

        errors.Add(errorMessage);
      }
      catch (Exception e)
      {
        _logger.LogError(e, logMessage, string.Join(", ", imagesIds));

        errors.Add(errorMessage);
      }

      return false;
    }

    public RemoveImagesCommand(
      IImageRepository imageRepository,
      ICertificateRepository certificateRepository,
      IEducationRepository educationRepository,
      IUserRepository userRepository,
      IHttpContextAccessor httpContextAccessor,
      IRequestClient<IRemoveImagesRequest> rcRemoveImages,
      IRemoveImagesRequestValidator removeRequestValidator,
      IAccessValidator accessValidator,
      ILogger<RemoveImagesCommand> logger)
    {
      _imageRepository = imageRepository;
      _certificateRepository = certificateRepository;
      _educationRepository = educationRepository;
      _userRepository = userRepository;
      _httpContextAccessor = httpContextAccessor;
      _rcRemoveImages = rcRemoveImages;
      _removeRequestValidator = removeRequestValidator;
      _accessValidator = accessValidator;
      _logger = logger;
    }

    public OperationResultResponse<bool> Execute(RemoveImagesRequest request)
    {
      OperationResultResponse<bool> response = new();

      Guid senderId = _httpContextAccessor.HttpContext.GetUserId();
      Guid? userId = null;

      switch (request.EntityType)
      {
        case EntityType.User:
          userId = request.EntityId;
          break;

        case EntityType.Certificate:
          userId = _certificateRepository.Get(request.EntityId).UserId;
          break;

        case EntityType.Education:
          userId = _educationRepository.Get(request.EntityId).UserId;
          break;
      }

      if (!_accessValidator.HasRights(senderId, Rights.AddEditRemoveUsers)
        && senderId != userId)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        response.Status = OperationResultStatusType.Failed;
        response.Errors.Add("Not enough rights.");

        return response;
      }

      if (!_removeRequestValidator.ValidateCustom(request, out List<string> errors))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        response.Status = OperationResultStatusType.Failed;
        response.Errors.AddRange(errors);

        return response;
      }

      response.Body = RemoveImages(request.ImagesIds, request.EntityId, response.Errors);

      if (response.Body)
      {
        DbUser dbUser = request.EntityType == EntityType.User
          ? null
          : _userRepository.Get(request.EntityId);

        if (dbUser != null
          && dbUser.AvatarFileId.HasValue
          && request.ImagesIds.Contains(dbUser.AvatarFileId.Value))
        {
          _userRepository.UpdateAvatar(request.EntityId, null);
        }

        _imageRepository.Remove(request.ImagesIds);
      }

      response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
