﻿using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Broker.Publishes.Interfaces;
using LT.DigitalOffice.UserService.Business.Commands.Avatar.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Avatar;
using LT.DigitalOffice.UserService.Validation.Image.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Avatar
{
  public class RemoveAvatarsCommand : IRemoveAvatarsCommand
  {
    private readonly IUserAvatarRepository _repository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRemoveAvatarsRequestValidator _validator;
    private readonly IAccessValidator _accessValidator;
    private readonly IResponseCreator _responseCreator;
    private readonly IGlobalCacheRepository _globalCache;
    private readonly IPublish _publish;

    public RemoveAvatarsCommand(
      IUserAvatarRepository repository,
      IHttpContextAccessor httpContextAccessor,
      IRemoveAvatarsRequestValidator validator,
      IAccessValidator accessValidator,
      IResponseCreator responseCreator,
      IGlobalCacheRepository globalCache,
      IPublish publish)
    {
      _repository = repository;
      _httpContextAccessor = httpContextAccessor;
      _validator = validator;
      _accessValidator = accessValidator;
      _responseCreator = responseCreator;
      _globalCache = globalCache;
      _publish = publish;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(RemoveAvatarsRequest request)
    {
      if (request.UserId != _httpContextAccessor.HttpContext.GetUserId()
        && !await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers))
      {
        _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator.ValidateAsync(request);

      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<bool>(
          HttpStatusCode.BadRequest,
          validationResult.Errors.Select(validationFailure => validationFailure.ErrorMessage).ToList());
      }
      OperationResultResponse<bool> response = new();

      response.Body = await _repository.RemoveAsync(request.AvatarsIds);

      if (response.Body)
      {
        await _publish.RemoveImagesAsync(imagesIds: request.AvatarsIds);
        await _globalCache.RemoveAsync(request.UserId);
      }
      else
      {
        response = _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest);
      }

      return response;
    }
  }
}
