using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using LT.DigitalOffice.UserService.Business.Commands.Avatar.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Avatar;
using LT.DigitalOffice.UserService.Validation.Image.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
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
    private readonly IImageService _imageService;
    private readonly IResponseCreator _responseCreator;

    public CreateAvatarCommand(
      IUserAvatarRepository avatarRepository,
      IAccessValidator accessValidator,
      ICreateAvatarRequestValidator requestValidator,
      IDbUserAvatarMapper dbEntityImageMapper,
      IHttpContextAccessor httpContextAccessor,
      IImageService imageService,
      IResponseCreator responseCreator)
    {
      _avatarRepository = avatarRepository;
      _accessValidator = accessValidator;
      _requestValidator = requestValidator;
      _dbUserAvatarMapper = dbEntityImageMapper;
      _httpContextAccessor = httpContextAccessor;
      _imageService = imageService;
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

      response.Body = await _imageService.CreateImageAsync(request, response.Errors);

      if (response.Body is not null)
      {
        await _avatarRepository.CreateAsync(
          _dbUserAvatarMapper
            .Map(response.Body.Value, request.UserId.Value, request.IsCurrentAvatar));

        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;
      }
      else
      {
        response = _responseCreator.CreateFailureResponse<Guid?>(HttpStatusCode.BadRequest);
      }

      return response;
    }
  }
}
