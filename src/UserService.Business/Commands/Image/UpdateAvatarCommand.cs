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
  public class UpdateAvatarCommand : IUpdateAvatarCommand
  {
    private readonly IImageRepository _imageRepository;
    private readonly IAccessValidator _accessValidator;
    private readonly IUpdateAvatarRequestValidator _requestValidator;
    private readonly IDbEntityImageMapper _dbEntityImageMapper;
    private readonly ICreateImageDataMapper _createImageDataMapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IResponseCreater _responseCreator;
    private readonly IRequestClient<ICreateImagesRequest> _rcCreateImage;
    private readonly ILogger<AddImagesCommand> _logger;

    private async Task<Guid?> AddImageAsync(string name, string content, string extension, List<string> errors)
    {
      const string errorMessage = "Can not add images. Please try again later.";
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

        errors.Add(errorMessage);
      }
      catch (Exception e)
      {
        _logger.LogError(e, logMessage);

        errors.Add(errorMessage);
      }

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
          new List<string>() { "Can't find image with sended Id."});
      }

      response.Status = OperationResultStatusType.FullSuccess;
      return response;
    }

    async Task<OperationResultResponse<Guid?>> UpdateByNewImage(Guid userId, string name, string content, string extension)
    {
      OperationResultResponse<Guid?> response = new();

      Guid? avatarImageId = await AddImageAsync(name, content, extension, response.Errors);

      if (!avatarImageId.HasValue)
      {
        return _responseCreator.CreateFailureResponse<Guid?>(HttpStatusCode.BadRequest, response.Errors);
      }

      response.Body = await _imageRepository.UpdateAvatarAsync(_dbEntityImageMapper.Map(avatarImageId.Value, userId, true));
      response.Status = OperationResultStatusType.FullSuccess;

      return response;
    }

    public UpdateAvatarCommand(
      ILogger<AddImagesCommand> logger,
      IHttpContextAccessor httpContextAccessor,
      IRequestClient<ICreateImagesRequest> rcCreateImage,
      IAccessValidator accessValidator,
      IDbEntityImageMapper dbEntityImageMapper,
      IUpdateAvatarRequestValidator requestValidator,
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
    public async Task<OperationResultResponse<Guid?>> ExecuteAsync(UpdateAvatarRequest request)
    {
      Guid senderId = _httpContextAccessor.HttpContext.GetUserId();

      if (!await _accessValidator.HasRightsAsync(senderId, Rights.AddEditRemoveUsers)
        && senderId != request.UserId)
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

      if (request.ImageId != null)
      {
        return await UpdateById(request.UserId, request.ImageId.Value);
      }
      else
      {
        return await UpdateByNewImage(request.UserId, request.Name, request.Content, request.Extension);
      }
    }
  }
}
