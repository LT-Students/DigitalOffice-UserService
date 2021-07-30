using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.File;
using LT.DigitalOffice.Models.Broker.Requests.Message;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.Models.Broker.Responses.File;
using LT.DigitalOffice.UserService.Business.Commands.Password.Interfaces;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
        private readonly IRequestClient<IAddImageRequest> _rcImage;
        private readonly IRequestClient<IChangeUserDepartmentRequest> _rcDepartment;
        private readonly IRequestClient<IChangeUserPositionRequest> _rcPosition;
        private readonly IRequestClient<IChangeUserRoleRequest> _rcRole;
        private readonly IRequestClient<IChangeUserOfficeRequest> _rcOffice;
        private readonly IRequestClient<ISendEmailRequest> _rcSendEmail;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGeneratePasswordCommand _generatePassword;

        #region private method

        private void ChangeUserDepartment(Guid departmentId, Guid userId, List<string> errors)
        {
            string errorMessage = $"Can't assign user {userId} to the department {departmentId}. Please try again later.";
            const string logMessage = "Can't assign user {UserId} to the department {DepartmentId}.";

            try
            {
                Response<IOperationResult<bool>> response = _rcDepartment.GetResponse<IOperationResult<bool>>(
                    IChangeUserDepartmentRequest.CreateObj(userId, departmentId)).Result;

                if (!response.Message.IsSuccess || !response.Message.Body)
                {
                    _logger.LogWarning(logMessage, userId, departmentId);

                    errors.Add(errorMessage);
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, logMessage, userId, departmentId);

                errors.Add(errorMessage);
            }
        }

        private void ChangeUserPosition(Guid positionId, Guid userId, List<string> errors)
        {
            string errorMessage = $"Can't assign position {positionId} to the user {userId}. Please try again later.";
            const string logMessage = "Can't assign position {PositionId} to the user {UserId}";

            try
            {
                Response<IOperationResult<bool>> response = _rcPosition.GetResponse<IOperationResult<bool>>(
                    IChangeUserPositionRequest.CreateObj(userId, positionId)).Result;

                if (!response.Message.IsSuccess || !response.Message.Body)
                {
                    _logger.LogWarning(logMessage, positionId, userId);

                    errors.Add(errorMessage);
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, logMessage, positionId, userId);

                errors.Add(errorMessage);
            }
        }

        private void ChangeUserRole(Guid roleId, Guid userId, List<string> errors)
        {
            string errorMessage = $"Can't assign role '{roleId}' to the user '{userId}'. Please try again later.";
            const string logMessage = "Can't assign role '{RoleId}' to the user '{UserId}'.";

            try
            {
                Response<IOperationResult<bool>> response = _rcRole.GetResponse<IOperationResult<bool>>(
                    IChangeUserRoleRequest.CreateObj(
                        roleId,
                        userId,
                        _httpContextAccessor.HttpContext.GetUserId())).Result;

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

        private void ChangeUserOffice(Guid officeId, Guid userId, List<string> errors)
        {
            string errorMessage = $"Can't assign office '{officeId}' to the user '{userId}'. Please try again later.";
            const string logMessage = "Can't assign office '{OfficeId}' to the user '{UserId}'.";

            try
            {
                Response<IOperationResult<bool>> response = _rcOffice.GetResponse<IOperationResult<bool>>(
                    IChangeUserOfficeRequest.CreateObj(
                        officeId,
                        userId,
                        _httpContextAccessor.HttpContext.GetUserId())).Result;
                if (!response.Message.IsSuccess || !response.Message.Body)
                {
                    const string warningMessage = logMessage + "Errors: {Errors}";
                    _logger.LogWarning(warningMessage, officeId, userId, string.Join("\n", response.Message.Errors));

                    errors.Add(errorMessage);
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, logMessage, officeId, userId);

                errors.Add(errorMessage);
            }
        }

        private Guid? GetAvatarImageId(AddImageRequest avatarRequest, List<string> errors)
        {
            Guid? avatarImageId = null;

            if (avatarRequest == null)
            {
                return null;
            }

            Guid userId = _httpContextAccessor.HttpContext.GetUserId();

            string errorMessage = $"Can not add avatar image to user with id {userId}. Please try again later.";

            try
            {
                var imageRequest = IAddImageRequest.CreateObj(
                    avatarRequest.Name,
                    avatarRequest.Content,
                    avatarRequest.Extension,
                    userId);
                var response = _rcImage.GetResponse<IOperationResult<IAddImageResponse>>(imageRequest).Result;

                if (!response.Message.IsSuccess)
                {
                    _logger.LogWarning(
                        "Can not add avatar image to user with id {UserId}. Reason: '{Errors}'",
                        userId,
                        string.Join(',', response.Message.Errors));

                    errors.Add(errorMessage);
                }
                else
                {
                    avatarImageId = response.Message.Body.Id;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Can not add avatar image to user with id {UserId}", userId);

                errors.Add(errorMessage);
            }

            return avatarImageId;
        }

        #endregion

        public EditUserCommand(
            IUserRepository userRepository,
            IUserCredentialsRepository credentialsRepository,
            IPatchDbUserMapper mapperUser,
            IAccessValidator accessValidator,
            ILogger<EditUserCommand> logger,
            IRequestClient<IAddImageRequest> rcImage,
            IRequestClient<IChangeUserDepartmentRequest> rcDepartment,
            IRequestClient<IChangeUserPositionRequest> rcPosition,
            IRequestClient<IChangeUserRoleRequest> rcRole,
            IRequestClient<IChangeUserOfficeRequest> rcOffice,
            IRequestClient<ISendEmailRequest> rcSendEmail,
            IHttpContextAccessor httpContextAccessor,
            IGeneratePasswordCommand generatePassword)
        {
            _userRepository = userRepository;
            _credentialsRepository = credentialsRepository;
            _mapperUser = mapperUser;
            _accessValidator = accessValidator;
            _logger = logger;
            _rcImage = rcImage;
            _rcDepartment = rcDepartment;
            _rcPosition = rcPosition;
            _rcRole = rcRole;
            _rcOffice = rcOffice;
            _rcSendEmail = rcSendEmail;
            _httpContextAccessor = httpContextAccessor;
            _generatePassword = generatePassword;
        }

        /// <inheritdoc/>
        public OperationResultResponse<bool> Execute(Guid userId, JsonPatchDocument<EditUserRequest> patch)
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

            if (!(_userRepository.Get(_httpContextAccessor.HttpContext.GetUserId()).IsAdmin ||
                _accessValidator.HasRights(Rights.AddEditRemoveUsers) ||
                (userId == _httpContextAccessor.HttpContext.GetUserId()
                && patch.Operations.FirstOrDefault(o => o.path.EndsWith(nameof(EditUserRequest.Rate), StringComparison.OrdinalIgnoreCase)) == null
                && positionOperation == null
                && departmentOperation == null
                && roleOperation == null
                && officeOperation == null)))
            {
                throw new ForbiddenException("Not enough rights.");
            }

            List<string> errors = new List<string>();

            Operation<EditUserRequest> imageOperation = patch.Operations.FirstOrDefault(
                o => o.path.EndsWith(nameof(EditUserRequest.AvatarImage), StringComparison.OrdinalIgnoreCase));
            Guid? imageId = null;

            if (imageOperation != null)
            {
                imageId = GetAvatarImageId(JsonConvert.DeserializeObject<AddImageRequest>(imageOperation.value?.ToString()), errors);
            }

            if (positionOperation != null)
            {
                ChangeUserPosition(Guid.Parse(positionOperation.value.ToString() ?? string.Empty), userId, errors);
            }

            if (departmentOperation != null)
            {
                ChangeUserDepartment(Guid.Parse(departmentOperation.value.ToString() ?? string.Empty), userId, errors);
            }

            if (roleOperation != null)
            {
                ChangeUserRole(Guid.Parse(roleOperation.value.ToString() ?? string.Empty), userId, errors);
            }

            if (officeOperation != null)
            {
                ChangeUserOffice(Guid.Parse(officeOperation.value.ToString() ?? string.Empty), userId, errors);
            }

            if (isActiveOperation != null)
            {
                _credentialsRepository.SwitchActiveStatus(
                    userId,
                    bool.Parse(isActiveOperation.value.ToString()));
            }

            var dbUserPatch = _mapperUser.Map(patch, imageId);
            _userRepository.EditUser(userId, dbUserPatch);

            return new OperationResultResponse<bool>
            {
                Status = errors.Any()
                    ? OperationResultStatusType.PartialSuccess
                    : OperationResultStatusType.FullSuccess,
                Body = true,
                Errors = errors
            };
        }

        // TODO fix
        //private void AddUserSkillsToDbUser(DbUser dbUser, CreateUserRequest request)
        //{
        //    if (request.Skills == null)
        //    {
        //        return;
        //    }

        //    foreach (var skillName in request.Skills)
        //    {
        //        var dbSkill = _userRepository.FindSkillByName(skillName);

        //        if (dbSkill != null)
        //        {
        //            dbUser.Skills.Add(
        //                new DbUserSkill
        //                {
        //                    Id = Guid.NewGuid(),
        //                    UserId = dbUser.Id,
        //                    SkillId = dbSkill.Id
        //                });
        //        }
        //        else
        //        {
        //            dbUser.Skills.Add(
        //                new DbUserSkill
        //                {
        //                    Id = Guid.NewGuid(),
        //                    UserId = dbUser.Id,
        //                    SkillId = _userRepository.CreateSkill(skillName)
        //                });
        //        }
        //    }
        //}
    }
}