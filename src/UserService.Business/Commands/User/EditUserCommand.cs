﻿using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Patch.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.User
{
  /// <inheritdoc/>
  public class EditUserCommand : IEditUserCommand
  {
    private readonly IUserRepository _userRepository;
    private readonly IUserCredentialsRepository _credentialsRepository;
    private readonly IPatchDbUserMapper _mapperUser;
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IResponseCreator _responseCreator;
    private readonly IGlobalCacheRepository _globalCache;
    private readonly IBus _bus;

    public EditUserCommand(
      IUserRepository userRepository,
      IUserCredentialsRepository credentialsRepository,
      IPatchDbUserMapper mapperUser,
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IResponseCreator responseCreator,
      IGlobalCacheRepository globalCache,
      IBus bus)
    {
      _userRepository = userRepository;
      _credentialsRepository = credentialsRepository;
      _mapperUser = mapperUser;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _responseCreator = responseCreator;
      _globalCache = globalCache;
      _bus = bus;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid userId, JsonPatchDocument<EditUserRequest> patch)
    {
      Guid requestSenderId = _httpContextAccessor.HttpContext.GetUserId();

      bool isAdmin = await _accessValidator.IsAdminAsync(userId);
      bool isAddEditRemoveUsers = await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers);

      Operation<EditUserRequest> isActiveOperation = patch.Operations.FirstOrDefault(
        o => o.path.EndsWith(nameof(EditUserRequest.IsActive), StringComparison.OrdinalIgnoreCase));

      Operation<EditUserRequest> isAdminOperation = patch.Operations.FirstOrDefault(
        o => o.path.EndsWith(nameof(EditUserRequest.IsAdmin), StringComparison.OrdinalIgnoreCase));

      Operation<EditUserRequest> isGenderOperation = patch.Operations.FirstOrDefault(
        o => o.path.EndsWith(nameof(EditUserRequest.GenderId), StringComparison.OrdinalIgnoreCase));

      if ((!isAddEditRemoveUsers && userId != requestSenderId && !isAdmin) ||
        (isActiveOperation is not null && !(isAddEditRemoveUsers || isAdmin)) || 
        (isAdminOperation is not null && !isAdmin) || 
        (isGenderOperation is not null && userId != requestSenderId))
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      OperationResultResponse<bool> response = new();

      if (isActiveOperation != null)
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

      (JsonPatchDocument<DbUser> dbUserPatch, JsonPatchDocument<DbUserAddition> dbUserAdditionPatch) = _mapperUser.Map(patch);

      if (dbUserPatch.Operations.Any())
      {
        response.Body = await _userRepository.EditUserAsync(userId, dbUserPatch);

        if (!response.Body)
        {
          return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.NotFound);
        }
      }

      if (dbUserAdditionPatch.Operations.Any())
      {
        response.Body = await _userRepository.EditUserAdditionAsync(userId, dbUserAdditionPatch);

        if (!response.Body)
        {
          return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.NotFound);
        }
      }

      await _globalCache.RemoveAsync(userId);

      return response;
    }
  }
}