using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Responses.Image;
using LT.DigitalOffice.UserService.Business.Commands.Avatar.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
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
  public class CreateAvatarCommand : ICreateAvatarCommand
  {
    private readonly IUserAvatarRepository _avatarRepository;
    private readonly IAccessValidator _accessValidator;
    private readonly ICreateAvatarRequestValidator _requestValidator;
    private readonly IDbUserAvatarMapper _dbUserAvatarMapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRequestClient<ICreateImagesRequest> _rcCreateImage;
    private readonly ILogger<CreateAvatarCommand> _logger;
    private readonly IResponseCreator _responseCreator;

    private async Task<Guid?> CreateImageAsync(string name, string content, string extension, List<string> errors)
    {
      const string logMessage = "Errors while adding images.";

      try
      {
        Response<IOperationResult<ICreateImagesResponse>> createResponse = await _rcCreateImage.GetResponse<IOperationResult<ICreateImagesResponse>>(
          ICreateImagesRequest.CreateObj(
            new List<CreateImageData>()
            {
              new CreateImageData(name, content, extension, _httpContextAccessor.HttpContext.GetUserId())
            },
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

    public CreateAvatarCommand(
      IUserAvatarRepository avatarRepository,
      IAccessValidator accessValidator,
      ICreateAvatarRequestValidator requestValidator,
      IDbUserAvatarMapper dbEntityImageMapper,
      IHttpContextAccessor httpContextAccessor,
      IRequestClient<ICreateImagesRequest> rcCreateImage,
      ILogger<CreateAvatarCommand> logger,
      IResponseCreator responseCreator)
    {
      _avatarRepository = avatarRepository;
      _accessValidator = accessValidator;
      _requestValidator = requestValidator;
      _dbUserAvatarMapper = dbEntityImageMapper;
      _httpContextAccessor = httpContextAccessor;
      _rcCreateImage = rcCreateImage;
      _logger = logger;
      _responseCreator = responseCreator;
    }

    public async Task<OperationResultResponse<Guid?>> ExecuteAsync(CreateAvatarRequest request)
    {
      if (_httpContextAccessor.HttpContext.GetUserId() != request.UserId
        && !await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers))
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

      OperationResultResponse<Guid?> response = new();

      response.Body = await CreateImageAsync(request.Name, request.Content, request.Extension, response.Errors);

      if (response.Body is not null)
      {
        DbUserAvatar dbUserAvatar = _dbUserAvatarMapper
          .Map(response.Body.Value, request.UserId.Value, request.IsCurrentAvatar);

        await _avatarRepository.CreateAsync(dbUserAvatar);

        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

        response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;
      }
      else
      {
        response = _responseCreator.CreateFailureResponse<Guid?>(HttpStatusCode.BadRequest);
      }

      return response;
    }
  }
}
