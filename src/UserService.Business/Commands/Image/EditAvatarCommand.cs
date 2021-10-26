using FluentValidation.Results;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
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
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Images;
using LT.DigitalOffice.UserService.Validation.Image.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Image
{
  public class EditAvatarCommand : IEditAvatarCommand
  {
    private readonly IImageRepository _imageRepository;
    private readonly IAccessValidator _accessValidator;
    private readonly IEditAvatarRequestValidator _requestValidator;
    private readonly IDbEntityImageMapper _dbEntityImageMapper;
    private readonly ICreateImageDataMapper _createImageDataMapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IResponseCreater _responseCreator;
    private readonly IRequestClient<ICreateImagesRequest> _rcCreateImage;
    private readonly ILogger<CreateImageCommand> _logger;

    private async Task<Guid?> AddImageAsync(string name, string content, string extension, List<string> errors)
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

    private async Task<OperationResultResponse<Guid?>> UpdateById(Guid userId, Guid imageId)
    {
      OperationResultResponse<Guid?> response = new();

      response.Body = await _imageRepository.UpdateAvatarAsync(userId, imageId);

      if (response.Body == null)
      {
        return _responseCreator.CreateFailureResponse<Guid?>(
          HttpStatusCode.NotFound,
          new List<string>() { "Can't find image with sended Id." });
      }

      response.Status = OperationResultStatusType.FullSuccess;
      return response;
    }

    public EditAvatarCommand(
      ILogger<CreateImageCommand> logger,
      IHttpContextAccessor httpContextAccessor,
      IRequestClient<ICreateImagesRequest> rcCreateImage,
      IAccessValidator accessValidator,
      IDbEntityImageMapper dbEntityImageMapper,
      IEditAvatarRequestValidator requestValidator,
      IResponseCreater responseCreator,
      ICreateImageDataMapper createImageDataMapper,
      IImageRepository imageRepository)
    {
      _logger = logger;
      _httpContextAccessor = httpContextAccessor;
      _rcCreateImage = rcCreateImage;
      _accessValidator = accessValidator;
      _dbEntityImageMapper = dbEntityImageMapper;
      _requestValidator = requestValidator;
      _responseCreator = responseCreator;
      _createImageDataMapper = createImageDataMapper;
      _imageRepository = imageRepository;
    }

    public async Task<OperationResultResponse<Guid?>> ExecuteAsync(Guid userId, Guid imageId)
    {
      Guid senderId = _httpContextAccessor.HttpContext.GetUserId();

      if (senderId != userId
        && !await _accessValidator.HasRightsAsync(senderId, Rights.AddEditRemoveUsers))
      {
        return _responseCreator.CreateFailureResponse<Guid?>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _requestValidator.ValidateAsync((userId, imageId));
      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<Guid?>(
          HttpStatusCode.BadRequest,
          validationResult.Errors.Select(validationFailure => validationFailure.ErrorMessage).ToList());
      }

      OperationResultResponse<Guid?> response = new();

      response.Body = await _imageRepository.UpdateAvatarAsync(userId, imageId);

      if (response.Body == null)
      {
        return _responseCreator.CreateFailureResponse<Guid?>(
          HttpStatusCode.NotFound,
          new List<string>() { "Can't find image with sended Id." });
      }

      response.Status = OperationResultStatusType.FullSuccess;
      return response;
    }
  }
}
