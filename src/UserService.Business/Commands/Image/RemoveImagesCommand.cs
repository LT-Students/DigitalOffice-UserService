using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.UserService.Business.Commands.Image.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
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
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Image
{
  public class RemoveImagesCommand : IRemoveImagesCommand
  {
    private readonly IImageRepository _imageRepository;
    private readonly ICertificateRepository _certificateRepository;
    private readonly IEducationRepository _educationRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRequestClient<IRemoveImagesRequest> _rcRemoveImages;
    private readonly IRemoveImagesRequestValidator _removeRequestValidator;
    private readonly IAccessValidator _accessValidator;
    private readonly ILogger<RemoveImagesCommand> _logger;

    private async Task<bool> RemoveImages(List<Guid> imagesIds, List<string> errors)
    {
      const string errorMessage = "Can't remove images. Please try again later.";
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

        errors.Add(errorMessage);
      }
      catch (Exception e)
      {
        _logger.LogError(e, logMessage, string.Join(", ", imagesIds));

        errors.Add(errorMessage);
      }

      return false;
    }

    private Guid? GetUserIdFromEntity(Guid entityId, EntityType entityType)
    {
      return entityType switch
      {
        EntityType.User => entityId,
        EntityType.Certificate => _certificateRepository.Get(entityId).UserId,
        EntityType.Education => _educationRepository.Get(entityId).UserId,
        _ => null
      };
    }

    public RemoveImagesCommand(
      IImageRepository imageRepository,
      ICertificateRepository certificateRepository,
      IEducationRepository educationRepository,
      IHttpContextAccessor httpContextAccessor,
      IRequestClient<IRemoveImagesRequest> rcRemoveImages,
      IRemoveImagesRequestValidator removeRequestValidator,
      IAccessValidator accessValidator,
      ILogger<RemoveImagesCommand> logger)
    {
      _imageRepository = imageRepository;
      _certificateRepository = certificateRepository;
      _educationRepository = educationRepository;
      _httpContextAccessor = httpContextAccessor;
      _rcRemoveImages = rcRemoveImages;
      _removeRequestValidator = removeRequestValidator;
      _accessValidator = accessValidator;
      _logger = logger;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(RemoveImagesRequest request)
    {
      OperationResultResponse<bool> response = new();

      Guid senderId = _httpContextAccessor.HttpContext.GetUserId();

      if (!await _accessValidator.HasRightsAsync(senderId, Rights.AddEditRemoveUsers)
        && senderId != GetUserIdFromEntity(request.EntityId, request.EntityType))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        response.Status = OperationResultStatusType.Failed;
        response.Errors.Add("Not enough rights.");

        return response;
      }

      ValidationResult validationResult = await _removeRequestValidator.ValidateAsync(request);

      if (!validationResult.IsValid)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        response.Status = OperationResultStatusType.Failed;
        response.Errors.AddRange(validationResult.Errors.Select(validationFailure => validationFailure.ErrorMessage).ToList());

        return response;
      }

      response.Body = await _imageRepository.RemoveAsync(request.ImagesIds);

      if (response.Body)
      {
        await RemoveImages(request.ImagesIds, response.Errors);
      }

      response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
