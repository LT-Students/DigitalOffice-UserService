using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.UserService.Business.Commands.User
{
  /// <inheritdoc/>
  public class EditUserCommand : IEditUserCommand
  {
    private readonly IUserRepository _userRepository;
    private readonly IUserCredentialsRepository _credentialsRepository;
    private readonly IPatchDbUserMapper _mapperUser;
    private readonly IAccessValidator _accessValidator;
    private readonly ILogger<EditUserCommand> _logger;
    private readonly IRequestClient<IEditCompanyEmployeeRequest> _rcEditCompanyEmployee;
    private readonly IRequestClient<IChangeUserRoleRequest> _rcRole;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IResponseCreater _responseCreater;
    private readonly ICacheNotebook _cacheNotebook;
    private readonly IBus _bus;

    #region private method

    private async Task EditCompanyEmployeeAsync(bool removeDepartment, Guid? departmentId, Guid? positionId, Guid? officeId, Guid userId, List<string> errors)
    {
      string departmentErrorMessage = $"Cannot assign position to user. Please try again later.";
      string positionErrorMessage = $"Cannot assign department to user. Please try again later.";
      string officeErrorMessage = $"Cannot assign office to user. Please try again later.";
      const string departmentLogMessage = "Cannot assign department {departmentId} to user with id {UserId}.";
      const string positionLogMessage = "Cannot assign position {positionId} to user with id {UserId}.";
      const string officeLogMessage = "Cannot assign office {officeId} to user with id {UserId}.";
      const string logMessage = "Cannot edit company employee info for user witd id {UserId}.";

      if (!removeDepartment && !departmentId.HasValue && !positionId.HasValue && !officeId.HasValue)
      {
        return;
      }

      try
      {
        IOperationResult<(bool department, bool position, bool office)> response =
          (await _rcEditCompanyEmployee.GetResponse<IOperationResult<(bool department, bool position, bool office)>>(
            IEditCompanyEmployeeRequest.CreateObj(
              userId,
              _httpContextAccessor.HttpContext.GetUserId(),
              removeUserFromDepartment: removeDepartment,
              departmentId: departmentId,
              positionId: positionId,
              officeId: officeId))).Message;

        if (!response.IsSuccess)
        {
          _logger.LogWarning(logMessage, userId);

          errors.Add(departmentErrorMessage);
          errors.Add(positionErrorMessage);
          errors.Add(officeErrorMessage);

          return;
        }

        if (!response.Body.department)
        {
          _logger.LogWarning(departmentLogMessage, userId, departmentId);

          errors.Add(departmentErrorMessage);
        }

        if (!response.Body.position)
        {
          _logger.LogWarning(positionLogMessage, userId, positionId);

          errors.Add(positionErrorMessage);
        }

        if (!response.Body.office)
        {
          _logger.LogWarning(officeLogMessage, userId, officeId);

          errors.Add(officeErrorMessage);
        }
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage, userId);

        errors.Add(departmentErrorMessage);
        errors.Add(positionErrorMessage);
        errors.Add(officeErrorMessage);
      }
    }

    private async Task ChangeUserRoleAsync(Guid roleId, Guid userId, List<string> errors)
    {
      string errorMessage = $"Can't assign role '{roleId}' to the user '{userId}'. Please try again later.";
      const string logMessage = "Can't assign role '{RoleId}' to the user '{UserId}'.";

      try
      {
        Response<IOperationResult<bool>> response = await _rcRole.GetResponse<IOperationResult<bool>>(
          IChangeUserRoleRequest.CreateObj(
            roleId, userId, _httpContextAccessor.HttpContext.GetUserId()));

        if (!response.Message.IsSuccess || !response.Message.Body)
        {
          const string warningMessage = logMessage + "Errors: {Errors}";
          _logger.LogWarning(warningMessage, roleId, userId, string.Join("\n", response.Message.Errors));

          errors.Add(errorMessage);
        }
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage, roleId, userId);

        errors.Add(errorMessage);
      }
    }
    #endregion

    public EditUserCommand(
      IUserRepository userRepository,
      IUserCredentialsRepository credentialsRepository,
      IPatchDbUserMapper mapperUser,
      IAccessValidator accessValidator,
      ILogger<EditUserCommand> logger,
      IRequestClient<IEditCompanyEmployeeRequest> rcEditCompanyEmployee,
      IRequestClient<IChangeUserRoleRequest> rcRole,
      IHttpContextAccessor httpContextAccessor,
      IResponseCreater responseCreater,
      ICacheNotebook cacheNotebook,
      IBus bus)
    {
      _userRepository = userRepository;
      _credentialsRepository = credentialsRepository;
      _mapperUser = mapperUser;
      _accessValidator = accessValidator;
      _logger = logger;
      _rcEditCompanyEmployee = rcEditCompanyEmployee;
      _rcRole = rcRole;
      _httpContextAccessor = httpContextAccessor;
      _responseCreater = responseCreater;
      _cacheNotebook = cacheNotebook;
      _bus = bus;
    }

    /// <inheritdoc/>
    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid userId, JsonPatchDocument<EditUserRequest> patch)
    {
      Operation<EditUserRequest> positionOperation = patch.Operations.FirstOrDefault(
        o => o.path.EndsWith(nameof(EditUserRequest.PositionId), StringComparison.OrdinalIgnoreCase));
      Operation<EditUserRequest> departmentOperation = patch.Operations.FirstOrDefault(
        o => o.path.EndsWith(nameof(EditUserRequest.DepartmentId), StringComparison.OrdinalIgnoreCase));
      Operation<EditUserRequest> roleOperation = patch.Operations.FirstOrDefault(
        o => o.path.EndsWith(nameof(EditUserRequest.RoleId), StringComparison.OrdinalIgnoreCase));
      Operation<EditUserRequest> officeOperation = patch.Operations.FirstOrDefault(
        o => o.path.EndsWith(nameof(EditUserRequest.OfficeId), StringComparison.OrdinalIgnoreCase));
      Operation<EditUserRequest> isActiveOperation = patch.Operations.FirstOrDefault(
        o => o.path.EndsWith(nameof(EditUserRequest.IsActive), StringComparison.OrdinalIgnoreCase));

      Guid requestSenderId = _httpContextAccessor.HttpContext.GetUserId();

      if (!((await _userRepository.GetAsync(_httpContextAccessor.HttpContext.GetUserId())).IsAdmin ||
        await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers) ||
        (userId == requestSenderId
        && patch.Operations.FirstOrDefault(o => o.path.EndsWith(nameof(EditUserRequest.Rate), StringComparison.OrdinalIgnoreCase)) == null
        && positionOperation == null
        && departmentOperation == null
        && roleOperation == null
        && officeOperation == null)))
      {
        _responseCreater.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      OperationResultResponse<bool> response = new();

      List<string> errors = new List<string>();

      Operation<EditUserRequest> imageOperation = patch.Operations.FirstOrDefault(
          o => o.path.EndsWith(nameof(EditUserRequest.AvatarFileId), StringComparison.OrdinalIgnoreCase));

      /*Guid? imageId = null;

      if (imageOperation != null)
      {
        imageId = !string.IsNullOrEmpty(imageOperation.value?.ToString())
          ? Guid.Parse(imageOperation.value?.ToString())
          : null;
      }*/

      bool removeUserFromDepartmen = departmentOperation != null && departmentOperation.value == null;
      Guid? newDepartmentId = null;
      Guid? newPositionId = null;
      Guid? newOfficeId = null;

      if (departmentOperation != null && departmentOperation.value != null)
      {
        newDepartmentId = Guid.Parse(departmentOperation?.value.ToString());
      }

      if (positionOperation != null)
      {
        newPositionId = Guid.Parse(positionOperation?.value.ToString());
      }

      if (officeOperation != null)
      {
        newOfficeId = Guid.Parse(officeOperation?.value.ToString());
      }

      await EditCompanyEmployeeAsync(
        removeUserFromDepartmen,
        newDepartmentId,
        newPositionId,
        newOfficeId,
        userId,
        errors);

      if (roleOperation != null)
      {
        await ChangeUserRoleAsync(Guid.Parse(roleOperation.value.ToString() ?? string.Empty), userId, errors);
      }

      if (isActiveOperation != null)
      {
        bool newValue = bool.Parse(isActiveOperation.value.ToString());

        bool switchActiveStatusResult = await _credentialsRepository.SwitchActiveStatusAsync(userId, newValue);

        if (!switchActiveStatusResult)
        {
          errors.Add("Can not change is active status.");
        }
        else if (!newValue)
        {
          await _bus.Publish<IDisactivateUserRequest>(IDisactivateUserRequest.CreateObj(
            userId,
            requestSenderId));
        }
      }

      response.Body = await _userRepository.EditUserAsync(userId, _mapperUser.Map(patch));

      await _cacheNotebook.RemoveAsync(userId);

      response.Status = errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;

      response.Errors.AddRange(errors);

      return response;
    }
  }
}