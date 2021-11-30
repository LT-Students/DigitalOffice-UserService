using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Image.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Validation.Image.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Image
{
  public class EditAvatarCommand : IEditAvatarCommand
  {
    private readonly IImageRepository _imageRepository;
    private readonly IAccessValidator _accessValidator;
    private readonly IEditAvatarRequestValidator _requestValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IResponseCreator _responseCreator;

    public EditAvatarCommand(
      IHttpContextAccessor httpContextAccessor,
      IAccessValidator accessValidator,
      IEditAvatarRequestValidator requestValidator,
      IResponseCreator responseCreator,
      IImageRepository imageRepository)
    {
      _httpContextAccessor = httpContextAccessor;
      _accessValidator = accessValidator;
      _requestValidator = requestValidator;
      _responseCreator = responseCreator;
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
