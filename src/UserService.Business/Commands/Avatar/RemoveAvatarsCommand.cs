using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.UserService.Business.Commands.Avatar.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Avatar;
using LT.DigitalOffice.UserService.Validation.Image.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Avatar
{
  public class RemoveAvatarsCommand : IRemoveAvatarsCommand
  {
    private readonly IAvatarRepository _avatarRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRequestClient<IRemoveImagesRequest> _rcRemoveImages;
    private readonly IRemoveAvatarsRequestValidator _removeRequestValidator;
    private readonly IAccessValidator _accessValidator;
    private readonly ILogger<RemoveAvatarsCommand> _logger;
    private readonly IResponseCreator _responseCreator;
    private readonly IGlobalCacheRepository _globalCache;

    private async Task<bool> RemoveImages(List<Guid> imagesIds, List<string> errors)
    {
      const string logMessage = "Errors while removing images with ids: {ImagesIds}.";

      try
      {
        Response<IOperationResult<bool>> removeResponse = await _rcRemoveImages.GetResponse<IOperationResult<bool>>(
            IRemoveImagesRequest.CreateObj(imagesIds, ImageSource.User));

        if (removeResponse.Message.IsSuccess)
        {
          return removeResponse.Message.Body;
        }

        _logger.LogWarning(
          logMessage + " Errors: {Errors}",
          string.Join(", ", imagesIds),
          string.Join('\n', removeResponse.Message.Errors));
      }
      catch (Exception e)
      {
        _logger.LogError(e, logMessage, string.Join(", ", imagesIds));
      }
      errors.Add("Cannot remove images. Please try again later.");

      return false;
    }

    public RemoveAvatarsCommand(
      IAvatarRepository imageRepository,
      IHttpContextAccessor httpContextAccessor,
      IRequestClient<IRemoveImagesRequest> rcRemoveImages,
      IRemoveAvatarsRequestValidator removeRequestValidator,
      IAccessValidator accessValidator,
      ILogger<RemoveAvatarsCommand> logger,
      IResponseCreator responseCreator,
      IGlobalCacheRepository globalCache)
    {
      _avatarRepository = imageRepository;
      _httpContextAccessor = httpContextAccessor;
      _rcRemoveImages = rcRemoveImages;
      _removeRequestValidator = removeRequestValidator;
      _accessValidator = accessValidator;
      _logger = logger;
      _responseCreator = responseCreator;
      _globalCache = globalCache;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(RemoveAvatarsRequest request)
    {
      if (request.UserId != _httpContextAccessor.HttpContext.GetUserId()
        && !await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers))
      {
        _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _removeRequestValidator.ValidateAsync(request);

      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<bool>(
          HttpStatusCode.BadRequest,
          validationResult.Errors.Select(validationFailure => validationFailure.ErrorMessage).ToList());
      }
      OperationResultResponse<bool> response = new();

      response.Body = await _avatarRepository.RemoveAsync(request.AvatarsIds);

      if (response.Body)
      {
        await RemoveImages(request.AvatarsIds, response.Errors);

        await _globalCache.RemoveAsync(request.UserId);
      }

      response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
