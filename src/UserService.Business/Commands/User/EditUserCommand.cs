using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Patch.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Validation.User.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.User
{
  /// <inheritdoc/>
  public class EditUserCommand : IEditUserCommand
  {
    private readonly IEditUserRequestValidator _validator;
    private readonly IUserRepository _userRepository;
    private readonly IUserCredentialsRepository _credentialsRepository;
    private readonly IUserLocationRepository _locationRepository;
    private readonly IPatchDbUserMapper _mapperUser;
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IResponseCreater _responseCreator;
    private readonly ICacheNotebook _cacheNotebook;
    private readonly IBus _bus;

    public EditUserCommand(
      IEditUserRequestValidator validator,
      IUserRepository userRepository,
      IUserCredentialsRepository credentialsRepository,
      IUserLocationRepository locationRepository,
      IPatchDbUserMapper mapperUser,
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IResponseCreater responseCreater,
      ICacheNotebook cacheNotebook,
      IBus bus)
    {
      _validator = validator;
      _userRepository = userRepository;
      _credentialsRepository = credentialsRepository;
      _locationRepository = locationRepository;
      _mapperUser = mapperUser;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _responseCreator = responseCreater;
      _cacheNotebook = cacheNotebook;
      _bus = bus;
    }

    /// <inheritdoc/>
    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid userId, JsonPatchDocument<EditUserRequest> patch)
    {
      bool genderOperation = patch.Operations.FirstOrDefault(
        o => o.path.EndsWith(nameof(EditUserRequest.Gender), StringComparison.OrdinalIgnoreCase)) is not null;

      bool startWorkingAtOperation = patch.Operations.FirstOrDefault(
        o => o.path.EndsWith(nameof(EditUserRequest.StartWorkingAt), StringComparison.OrdinalIgnoreCase)) is not null;

      bool isAdminOperation = patch.Operations.FirstOrDefault(
        o => o.path.EndsWith(nameof(EditUserRequest.IsAdmin), StringComparison.OrdinalIgnoreCase)) is not null;

      Operation<EditUserRequest> isActiveOperation = patch.Operations.FirstOrDefault(
        o => o.path.EndsWith(nameof(EditUserRequest.IsActive), StringComparison.OrdinalIgnoreCase));

      Guid requestSenderId = _httpContextAccessor.HttpContext.GetUserId();

      if ((userId != requestSenderId && !await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers))
        || (userId == requestSenderId && (startWorkingAtOperation || isAdminOperation || isActiveOperation is not null))
        || (userId != requestSenderId && genderOperation)
        || isAdminOperation && !await _accessValidator.IsAdminAsync())
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      if (!_validator.ValidateCustom(patch, out List<string> errors))
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest, errors);
      }

      bool latitudeOperation = patch.Operations.FirstOrDefault(
        o => o.path.EndsWith(nameof(EditUserRequest.Latitude), StringComparison.OrdinalIgnoreCase)) is not null;
      bool longitudeOperation = patch.Operations.FirstOrDefault(
        o => o.path.EndsWith(nameof(EditUserRequest.Longitude), StringComparison.OrdinalIgnoreCase)) is not null;

      if ((latitudeOperation || longitudeOperation) && !(latitudeOperation && longitudeOperation))
      {
        return _responseCreator.CreateFailureResponse<bool>(
          HttpStatusCode.BadRequest,
          new() { "latitude and longitude must have a value." });
      }

      OperationResultResponse<bool> response = new();

      if (isActiveOperation is not null)
      {
        bool newValue = bool.Parse(isActiveOperation.value.ToString());
        bool switchActiveStatusResult = await _credentialsRepository.SwitchActiveStatusAsync(userId, newValue);

        if (!switchActiveStatusResult)
        {
          response.Errors.Add("Can not change is active status.");
        }
        else if (!newValue)
        {
          await _bus.Publish<IDisactivateUserRequest>(IDisactivateUserRequest.CreateObj(
            userId,
            requestSenderId));
        }
      }

      (JsonPatchDocument<DbUser> dbUserPatch, JsonPatchDocument<DbUserLocation> dbUserLocationPatch) = _mapperUser.Map(patch);

      if (dbUserPatch.Operations.Any())
      {
        response.Body = await _userRepository.EditUserAsync(userId, dbUserPatch);

        if (!response.Body)
        {
          return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest);
        }
      }

      if (dbUserLocationPatch.Operations.Any())
      {
        response.Body = await _locationRepository.EditAsync(userId, dbUserLocationPatch);

        response.Errors = response.Body ? new() : new() { "Cannot edit user location." };
      }

      await _cacheNotebook.RemoveAsync(userId);

      response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}