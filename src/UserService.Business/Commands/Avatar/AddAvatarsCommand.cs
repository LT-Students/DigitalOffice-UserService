using FluentValidation.Results;
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
using LT.DigitalOffice.UserService.Business.Commands.Avatar.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Avatars;
using LT.DigitalOffice.UserService.Validation.Avatars.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace LT.DigitalOffice.UserService.Business.Commands.Avatar
{
  public class AddAvatarsCommand : IAddAvatarsCommand
  {
    private readonly IAvatarRepository _avatarRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAccessValidator _accessValidator;
    private readonly IAddAvatarRequestValidator _requestValidator;
    private readonly IDbUserAvatarMapper _dbUserAvatarMapper;
    private readonly ICreateImageDataMapper _createImageDataMapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRequestClient<ICreateImagesRequest> _rcCreateImage;
    private readonly ILogger<AddAvatarsCommand> _logger;

    public AddAvatarsCommand(
      IAvatarRepository avatarRepository,
      IUserRepository userRepository,
      IAccessValidator accessValidator,
      IAddAvatarRequestValidator requestValidator,
      IDbUserAvatarMapper dbUserAvatarMapper,
      ICreateImageDataMapper createImageDataMapper,
      IHttpContextAccessor httpContextAccessor,
      IRequestClient<ICreateImagesRequest> rcCreateImage,
      ILogger<AddAvatarsCommand> logger)
    {
      _avatarRepository = avatarRepository;
      _userRepository = userRepository;
      _accessValidator = accessValidator;
      _requestValidator = requestValidator;
      _dbUserAvatarMapper = dbUserAvatarMapper;
      _createImageDataMapper = createImageDataMapper;
      _httpContextAccessor = httpContextAccessor;
      _rcCreateImage = rcCreateImage;
      _logger = logger;
    }

    private List<Guid> AddImages(List<AddImageRequest> request, Guid senderId, List<string> errors)
    {
      if (request == null || request.Contains(null))
      {
        return null;
      }

      string errorMessage = "Can not add images. Please try again later.";
      string logMessage = "Errors while adding images.";

      try
      {
        IOperationResult<ICreateImagesResponse> createResponse = _rcCreateImage.GetResponse<IOperationResult<ICreateImagesResponse>>(
          ICreateImagesRequest.CreateObj(
            _createImageDataMapper.Map(request, senderId),
            ImageSource.User))
          .Result.Message;

        if (createResponse.IsSuccess)
        {
          return createResponse.Body.ImagesIds;
        }

        string warningMessage = logMessage + " Errors: {Errors}";

        _logger.LogWarning(
          warningMessage,
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

    private List<Guid> AddImagesToDb(List<Guid> imageIds, Guid userId)
    {
      List<DbUserAvatar> dbUsersAvatarsCreate = _dbUserAvatarMapper.Map(imageIds, userId);

      return _avatarRepository.Create(dbUsersAvatarsCreate);
    }

    public OperationResultResponse<List<Guid>> Execute(AddAvatarRequest request)
    {
      OperationResultResponse<List<Guid>> response = new();

      Guid senderId = _httpContextAccessor.HttpContext.GetUserId();

      if (!_accessValidator.HasRights(senderId, Rights.AddEditRemoveUsers)
        && senderId != request.UserId)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        response.Status = OperationResultStatusType.Failed;
        response.Errors.Add("Not enough rights.");

        return response;
      }

      List<string> errors = new();

      if (!_requestValidator.ValidateCustom(request, out errors))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        response.Status = OperationResultStatusType.Failed;
        response.Errors.AddRange(errors);

        return response;
      }

      List<Guid> result = AddImages(request.Images, senderId, response.Errors);

      if (result != null)
      {
        AddImagesToDb(result, request.UserId);
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
