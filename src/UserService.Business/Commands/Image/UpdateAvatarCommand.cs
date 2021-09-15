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
  public class UpdateAvatarCommand : IUpdateAvatarCommand
  {
    private readonly IImageRepository _imageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAccessValidator _accessValidator;
    private readonly IUpdateAvatarRequestValidator _requestValidator;
    private readonly IDbEntityImageMapper _dbEntityImageMapper;
    private readonly ICreateImageDataMapper _createImageDataMapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRequestClient<ICreateImagesRequest> _rcCreateImage;
    private readonly ILogger<AddImagesCommand> _logger;

    private Guid? AddImage(UpdateAvatarRequest request, Guid senderId, List<string> errors)
    {
      string errorMessage = "Can not add image. Please try again later.";
      string logMessage = "Errors while adding image.";

      try
      {
        IOperationResult<ICreateImagesResponse> createResponse = _rcCreateImage.GetResponse<IOperationResult<ICreateImagesResponse>>(
          ICreateImagesRequest.CreateObj(
            _createImageDataMapper.Map(request.Name, request.Content, request.Extension, senderId),
            ImageSource.User))
          .Result.Message;

        if (createResponse.IsSuccess)
        {
          return createResponse.Body.ImagesIds.FirstOrDefault();
        }

        _logger.LogWarning(
          logMessage + " Errors: {Errors}",
          string.Join('\n', createResponse.Errors));

        errors.Add(errorMessage);
      }
      catch (Exception e)
      {
        _logger.LogError(e, logMessage);

        errors.Add(errorMessage);
      }

      return null;
    }

    public UpdateAvatarCommand(
      IImageRepository imageRepository,
      IUserRepository userRepository,
      IAccessValidator accessValidator,
      IUpdateAvatarRequestValidator requestValidator,
      IDbEntityImageMapper dbEntityImageMapper,
      ICreateImageDataMapper createImageDataMapper,
      IHttpContextAccessor httpContextAccessor,
      IRequestClient<ICreateImagesRequest> rcCreateImage,
      ILogger<AddImagesCommand> logger)
    {
      _imageRepository = imageRepository;
      _userRepository = userRepository;
      _accessValidator = accessValidator;
      _requestValidator = requestValidator;
      _dbEntityImageMapper = dbEntityImageMapper;
      _createImageDataMapper = createImageDataMapper;
      _httpContextAccessor = httpContextAccessor;
      _rcCreateImage = rcCreateImage;
      _logger = logger;
    }

    public OperationResultResponse<Guid?> Execute(UpdateAvatarRequest request)
    {
      OperationResultResponse<Guid?> response = new();

      Guid senderId = _httpContextAccessor.HttpContext.GetUserId();

      if (!_accessValidator.HasRights(senderId, Rights.AddEditRemoveUsers)
        && senderId != request.UserId)
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

      Guid? result = AddImage(request, senderId, response.Errors);

      if (result.HasValue)
      {
        List<DbEntityImage> dbEntityImages = _dbEntityImageMapper.Map(new List<Guid> { result.Value }, request.UserId);
        _userRepository.UpdateAvatar(request.UserId, result);

        result = _imageRepository.Create(dbEntityImages).FirstOrDefault();
      }

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;
      response.Body = result;
      response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
