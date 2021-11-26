﻿using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Communication.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Communication
{
  public class RemoveCommunicationCommand : IRemoveCommunicationCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICommunicationRepository _repository;
    private readonly IResponseCreater _responseCreator;

    public RemoveCommunicationCommand(
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      ICommunicationRepository communicationRepository,
      IResponseCreater responseCreator)
    {
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _repository = communicationRepository;
      _responseCreator = responseCreator;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid communicationId)
    {
      DbUserCommunication dbUserCommunication = await _repository.GetAsync(communicationId);

      if ((_httpContextAccessor.HttpContext.GetUserId() != dbUserCommunication.UserId) &&
        !await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers))
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      if (dbUserCommunication.Type == (int)CommunicationType.Email &&
        (await _repository.CountUserEmails(dbUserCommunication.UserId)) < 2)
      {
        return _responseCreator.CreateFailureResponse<bool>(
          HttpStatusCode.BadRequest,
          new List<string>() { "Last email can not be removed." });
      }

      bool result = await _repository.RemoveAsync(dbUserCommunication);

      return new OperationResultResponse<bool>
      {
        Status = result ? OperationResultStatusType.FullSuccess : OperationResultStatusType.Failed,
        Body = result
      };
    }
  }
}
