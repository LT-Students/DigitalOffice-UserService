using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
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
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
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
  public class AddImagesCommand : IAddImagesCommand
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
    private readonly ILogger<AddImagesCommand> _logger;

    private async Task<List<Guid>> AddImages(List<AddImageRequest> request, Guid senderId, List<string> errors)
    {
      if (request == null || !request.Any())
      {
        return null;
      }

      const string errorMessage = "Can not add images. Please try again later.";
      const string logMessage = "Errors while adding images.";

      try
      {
        Response<IOperationResult<ICreateImagesResponse>> createResponse = await _rcCreateImage.GetResponse<IOperationResult<ICreateImagesResponse>>(
          ICreateImagesRequest.CreateObj(
            _createImageDataMapper.Map(request, senderId),
            ImageSource.User));

        if (createResponse.Message.IsSuccess)
        {
          return createResponse.Message.Body.ImagesIds;
        }

        _logger.LogWarning(
          logMessage + " Errors: {Errors}",
          string.Join('\n', createResponse.Message.Errors));

        errors.Add(errorMessage);
      }
      catch (Exception e)
      {
        _logger.LogError(e, logMessage);

        errors.Add(errorMessage);
      }

      return null;
    }

    private Guid? GetUserIdFromEntity(Guid entityId, EntityType entityType)
    {
      Guid? userId = null;

      switch (entityType)
      {
        case EntityType.User:
          userId = entityId;
          break;

        case EntityType.Certificate:
          userId = _certificateRepository.Get(entityId).UserId;
          break;

        case EntityType.Education:
          userId = _educationRepository.Get(entityId).UserId;
          break;
      }

      return userId;
    }

    public AddImagesCommand(
      IImageRepository imageRepository,
      ICertificateRepository certificateRepository,
      IEducationRepository educationRepository,
      IAccessValidator accessValidator,
      IAddImagesRequestValidator requestValidator,
      IDbEntityImageMapper dbEntityImageMapper,
      ICreateImageDataMapper createImageDataMapper,
      IHttpContextAccessor httpContextAccessor,
      IRequestClient<ICreateImagesRequest> rcCreateImage,
      ILogger<AddImagesCommand> logger)
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
    }

    public async Task<OperationResultResponse<List<Guid>>> Execute(AddImagesRequest request)
    {
      OperationResultResponse<List<Guid>> response = new();

      Guid senderId = _httpContextAccessor.HttpContext.GetUserId();
      Guid? userId = GetUserIdFromEntity(request.EntityId, request.EntityType);

      if (!_accessValidator.HasRights(senderId, Rights.AddEditRemoveUsers)
        && senderId != userId)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        response.Status = OperationResultStatusType.Failed;
        response.Errors.Add("Not enough rights.");

        return response;
      }

      if (!_requestValidator.ValidateCustom(request, out List<string> errors))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        response.Status = OperationResultStatusType.Failed;
        response.Errors.AddRange(errors);

        return response;
      }

      List<Guid> result = await AddImages(request.Images, senderId, response.Errors);

      if (result != null)
      {
        List<DbEntityImage> dbEntityImages = _dbEntityImageMapper.Map(result, request.EntityId);

        result = _imageRepository.Create(dbEntityImages);
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;
      }
      else
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadGateway;
      }

      response.Body = result;
      response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
