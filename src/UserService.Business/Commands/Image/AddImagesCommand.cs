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

    private async Task<List<Guid>> AddImages(List<AddImageRequest> request, List<string> errors)
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
            _createImageDataMapper.Map(request),
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
      return entityType switch
      {
        EntityType.User => entityId,
        EntityType.Certificate => _certificateRepository.Get(entityId).UserId,
        EntityType.Education => _educationRepository.Get(entityId).UserId,
        _ => null
      };
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

      if (!_accessValidator.HasRights(senderId, Rights.AddEditRemoveUsers)
        && senderId != GetUserIdFromEntity(request.EntityId, request.EntityType))
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

      response.Body = await AddImages(request.Images, response.Errors);

      if (response.Body != null)
      {
        List<DbEntityImage> dbEntityImages = _dbEntityImageMapper.Map(response.Body, request.EntityId);

        _imageRepository.Create(dbEntityImages);

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
