using FluentValidation.Results;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Responses.Image;
using LT.DigitalOffice.UserService.Business.Commands.Image.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
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
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Image
{
  public class CreateImageCommand : ICreateImageCommand
  {
    private readonly IImageRepository _imageRepository;
    private readonly ICertificateRepository _certificateRepository;
    private readonly IEducationRepository _educationRepository;
    private readonly IAccessValidator _accessValidator;
    private readonly IAddImagesRequestValidator _requestValidator;
    private readonly IDbEntityImageMapper _dbEntityImageMapper;
    private readonly ICreateImageDataMapper _createImageDataMapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRequestClient<ICreateImagesRequest> _rcCreateImage;
    private readonly ILogger<CreateImageCommand> _logger;
    private readonly IResponseCreater _responseCreator;

    private async Task<Guid?> CreateImage(string name, string content, string extension, List<string> errors)
    {
      const string logMessage = "Errors while adding images.";

      try
      {
        Response<IOperationResult<ICreateImagesResponse>> createResponse = await _rcCreateImage.GetResponse<IOperationResult<ICreateImagesResponse>>(
          ICreateImagesRequest.CreateObj(
            _createImageDataMapper.Map(name, content, extension),
            ImageSource.User));

        if (createResponse.Message.IsSuccess
          && createResponse.Message.Body != null
          && createResponse.Message.Body.ImagesIds != null)
        {
          return createResponse.Message.Body.ImagesIds.FirstOrDefault();
        }

        _logger.LogWarning(
          logMessage + " Errors: {Errors}",
          string.Join('\n', createResponse.Message.Errors));
      }
      catch (Exception e)
      {
        _logger.LogError(e, logMessage);
      }

      errors.Add("Can not add images. Please try again later.");

      return null;
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

    public CreateImageCommand(
      IImageRepository imageRepository,
      ICertificateRepository certificateRepository,
      IEducationRepository educationRepository,
      IAccessValidator accessValidator,
      IAddImagesRequestValidator requestValidator,
      IDbEntityImageMapper dbEntityImageMapper,
      ICreateImageDataMapper createImageDataMapper,
      IHttpContextAccessor httpContextAccessor,
      IRequestClient<ICreateImagesRequest> rcCreateImage,
      ILogger<CreateImageCommand> logger,
      IResponseCreater responseCreator)
    {
      _imageRepository = imageRepository;
      _certificateRepository = certificateRepository;
      _educationRepository = educationRepository;
      _accessValidator = accessValidator;
      _requestValidator = requestValidator;
      _dbEntityImageMapper = dbEntityImageMapper;
      _createImageDataMapper = createImageDataMapper;
      _httpContextAccessor = httpContextAccessor;
      _rcCreateImage = rcCreateImage;
      _logger = logger;
      _responseCreator = responseCreator;
    }

    public async Task<OperationResultResponse<Guid?>> ExecuteAsync(CreateImageRequest request)
    {
      OperationResultResponse<Guid?> response = new();

      Guid senderId = _httpContextAccessor.HttpContext.GetUserId();

      if (senderId != GetUserIdFromEntity(request.EntityId, request.EntityType)
        && !await _accessValidator.HasRightsAsync(senderId, Rights.AddEditRemoveUsers))
      {
        return _responseCreator.CreateFailureResponse<Guid?>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _requestValidator.ValidateAsync(request);
      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<Guid?>(
          HttpStatusCode.BadRequest,
          validationResult.Errors.Select(validationFailure => validationFailure.ErrorMessage).ToList());
      }

      response.Body = await CreateImage(request.Name, request.Content, request.Extension, response.Errors);

      if (response.Body != null)
      {
        DbEntityImage dbEntityImage = _dbEntityImageMapper.Map(response.Body.Value, request.EntityId, request.IsCurrentAvatar);

        await _imageRepository.CreateAsync(dbEntityImage);

        if (request.EntityType == EntityType.User && request.IsCurrentAvatar)
        {
          await _imageRepository.UpdateAvatarAsync(request.EntityId, dbEntityImage.ImageId);
        }

        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

        response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;
      }
      else
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadGateway;
        response.Status = OperationResultStatusType.Failed;
      }

      return response;
    }
  }
}
