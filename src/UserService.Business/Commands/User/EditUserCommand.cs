using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.File;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.Models.Broker.Responses.File;
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
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Business
{
    /// <inheritdoc/>
    public class EditUserCommand : IEditUserCommand
    {
        private readonly IUserRepository _userRepository;
        private readonly IPatchDbUserMapper _mapperUser;
        private readonly IAccessValidator _accessValidator;
        private readonly ILogger<EditUserCommand> _logger;
        private readonly IRequestClient<IAddImageRequest> _rcImage;
        private readonly IRequestClient<IChangeUserDepartmentRequest> _rcDepartment;
        private readonly IRequestClient<IChangeUserPositionRequest> _rcPosition;
        private readonly IRequestClient<IChangeUserRoleRequest> _rcRole;
        private readonly IRequestClient<IChangeUserOfficeRequest> _rcOffice;
        private readonly IHttpContextAccessor _httpContextAccessor;

        #region private method

        private bool ChangeUserDepartment(Guid departmentId, Guid userId, List<string> errors)
        {
            string errorMessage = $"Сan't assign user {userId} to the department {departmentId}. Please try again later.";
            string logMessage = "Сan't assign user {userId} to the department {departmentId}.";

            try
            {
                Response<IOperationResult<bool>> response = _rcDepartment.GetResponse<IOperationResult<bool>>(
                    IChangeUserDepartmentRequest.CreateObj(userId, departmentId)).Result;

                if (response.Message.IsSuccess && response.Message.Body)
                {
                    return true;
                }

                _logger.LogWarning(logMessage, userId, departmentId);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, logMessage, userId, departmentId);
            }

            errors.Add(errorMessage);

            return false;
        }

        private bool ChangeUserPosition(Guid positionId, Guid userId, List<string> errors)
        {
            string errorMessage = $"Сan't assign position {positionId} to the user {userId}. Please try again later.";
            string logMessage = "Сan't assign position {positionId} to the user {userId}";

            try
            {
                Response<IOperationResult<bool>> response = _rcPosition.GetResponse<IOperationResult<bool>>(
                    IChangeUserPositionRequest.CreateObj(userId, positionId)).Result;

                if (response.Message.IsSuccess && response.Message.Body)
                {
                    return true;
                }

                _logger.LogWarning(logMessage, positionId, userId);
            }
            catch (Exception exc)
            {
                _logger.LogWarning(exc, logMessage, positionId, userId);
            }

            errors.Add(errorMessage);

            return false;
        }

        private bool ChangeUserRole(Guid roleId, Guid userId, List<string> errors)
        {
            string errorMessage = $"Сan't assign role '{roleId}' to the user '{userId}'. Please try again later.";
            string logMessage = "Сan't assign role '{roleId}' to the user '{userId}'.";

            try
            {
                Response<IOperationResult<bool>> response = _rcRole.GetResponse<IOperationResult<bool>>(
                    IChangeUserRoleRequest.CreateObj(
                        roleId,
                        userId,
                        _httpContextAccessor.HttpContext.GetUserId())).Result;

                if (response.Message.IsSuccess && response.Message.Body)
                {
                    return true;
                }

                _logger.LogWarning(logMessage + " Errors: {errors}", roleId, userId, string.Join("\n", response.Message.Errors));
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, logMessage, roleId, userId);
            }

            errors.Add(errorMessage);

            return false;
        }

        private bool ChangeUserOffice(Guid officeId, Guid userId, List<string> errors)
        {
            string errorMessage = $"Сan't assign office '{officeId}' to the user '{userId}'. Please try again later.";
            string logMessage = "Сan't assign office '{officeId}' to the user '{userId}'.";

            try
            {
                Response<IOperationResult<bool>> response = _rcOffice.GetResponse<IOperationResult<bool>>(
                    IChangeUserOfficeRequest.CreateObj(
                        officeId,
                        userId,
                        _httpContextAccessor.HttpContext.GetUserId())).Result;
                if (response.Message.IsSuccess && response.Message.Body)
                {
                    return true;                    
                }

                _logger.LogWarning(logMessage + " Errors: {errors}", officeId, userId, string.Join("\n", response.Message.Errors));
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, logMessage, officeId, userId);
            }

            errors.Add(errorMessage);

            return false;
        }

        private Guid? GetAvatarImageId(AddImageRequest avatarRequest, List<string> errors)
        {
            Guid? avatarImageId = null;

            if (avatarRequest == null)
            {
                return avatarImageId;
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
                var response = _rcImage.GetResponse<IOperationResult<IAddImageResponse>>(imageRequest, timeout: TimeSpan.FromSeconds(2)).Result;

                if (!response.Message.IsSuccess)
                {
                    _logger.LogWarning(
                        "Can not add avatar image to user with id {userId}." + $"Reason: '{string.Join(',', response.Message.Errors)}'", userId);

                    errors.Add(errorMessage);
                }
                else
                {
                    avatarImageId = response.Message.Body.Id;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Can not add avatar image to user with id {userId}.", userId);

                errors.Add(errorMessage);
            }

            return avatarImageId;
        }

        #endregion

        public EditUserCommand(
            ILogger<EditUserCommand> logger,
            IRequestClient<IAddImageRequest> rcImage,
            IHttpContextAccessor httpContextAccessor,
            IUserRepository userRepository,
            IPatchDbUserMapper mapperUser,
            IAccessValidator accessValidator,
            IRequestClient<IChangeUserDepartmentRequest> rcDepartment,
            IRequestClient<IChangeUserPositionRequest> rcPosition,
            IRequestClient<IChangeUserRoleRequest> rcRole,
            IRequestClient<IChangeUserOfficeRequest> rcOffice)
        {
            _logger = logger;
            _rcImage = rcImage;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _mapperUser = mapperUser;
            _accessValidator = accessValidator;
            _rcDepartment = rcDepartment;
            _rcPosition = rcPosition;
            _rcRole = rcRole;
            _rcOffice = rcOffice;
        }

        /// <inheritdoc/>
        public OperationResultResponse<bool> Execute(Guid userId, JsonPatchDocument<EditUserRequest> patch)
        {
            var status = OperationResultStatusType.FullSuccess;

            Operation<EditUserRequest> positionOperation = patch.Operations.FirstOrDefault(
                o => o.path.EndsWith(nameof(EditUserRequest.PositionId), StringComparison.OrdinalIgnoreCase)); ;
            Operation<EditUserRequest> departmentOperation = patch.Operations.FirstOrDefault(
                o => o.path.EndsWith(nameof(EditUserRequest.DepartmentId), StringComparison.OrdinalIgnoreCase));
            Operation<EditUserRequest> roleOperation = patch.Operations.FirstOrDefault(
                o => o.path.EndsWith(nameof(EditUserRequest.RoleId), StringComparison.OrdinalIgnoreCase));
            Operation<EditUserRequest> officeOperation = patch.Operations.FirstOrDefault(
                o => o.path.EndsWith(nameof(EditUserRequest.OfficeId), StringComparison.OrdinalIgnoreCase));

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
                if (imageId == null)
                {
                    status = OperationResultStatusType.PartialSuccess;
                }
            }

            if (positionOperation != null)
            {
                if (!ChangeUserPosition(Guid.Parse(positionOperation.value.ToString()), userId, errors))
                {
                    status = OperationResultStatusType.PartialSuccess;
                }
            }

            if (departmentOperation != null)
            {
                if (!ChangeUserDepartment(Guid.Parse(departmentOperation.value.ToString()), userId, errors))
                {
                    status = OperationResultStatusType.PartialSuccess;
                }
            }

            if (roleOperation != null)
            {
                if (!ChangeUserRole(Guid.Parse(roleOperation.value.ToString()), userId, errors))
                {
                    status = OperationResultStatusType.PartialSuccess;
                }
            }

            if (officeOperation != null)
            {
                if (!ChangeUserOffice(Guid.Parse(officeOperation.value.ToString()), userId, errors))
                {
                    status = OperationResultStatusType.PartialSuccess;
                }
            }

            var dbUserPatch = _mapperUser.Map(patch, imageId);
            _userRepository.EditUser(userId, dbUserPatch);

            return new OperationResultResponse<bool>
            {
                Status = status,
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