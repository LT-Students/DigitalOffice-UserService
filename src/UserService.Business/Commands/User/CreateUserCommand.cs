using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.File;
using LT.DigitalOffice.Models.Broker.Requests.Message;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.Models.Broker.Responses.File;
using LT.DigitalOffice.UserService.Business.Commands.Password.Interfaces;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Validation.User.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.UserService.Business.Commands.User
{
    /// <inheritdoc/>
    public class CreateUserCommand : ICreateUserCommand
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CreateUserCommand> _logger;
        private readonly IRequestClient<IAddImageRequest> _rcImage;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRequestClient<IChangeUserDepartmentRequest> _rcDepartment;
        private readonly IRequestClient<IChangeUserPositionRequest> _rcPosition;
        private readonly IRequestClient<IChangeUserRoleRequest> _rcRole;
        private readonly IRequestClient<IChangeUserOfficeRequest> _rcOffice;
        private readonly IRequestClient<ISendEmailRequest> _rcSendEmail;
        private readonly ICreateUserRequestValidator _validator;
        private readonly IDbUserMapper _mapperUser;
        private readonly IAccessValidator _accessValidator;
        private readonly IGeneratePasswordCommand _generatePassword;

        #region private methods

        private void ChangeUserDepartment(Guid departmentId, Guid userId, List<string> errors)
        {
            string errorMessage = $"Can't assign user {userId} to the department {departmentId}. Please try again later.";
            const string logMessage = "Can't assign user {UserId} to the department {DepartmentId}.";

            try
            {
                var response = _rcDepartment.GetResponse<IOperationResult<bool>>(
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

        private bool ChangeUserPosition(Guid positionId, Guid userId, List<string> errors)
        {
            string errorMessage = $"Can't assign position {positionId} to the user {userId}. Please try again later.";
            const string logMessage = "Can't assign position {PositionId} to the user {UserId}";

            try
            {
                var response = _rcPosition.GetResponse<IOperationResult<bool>>(
                    IChangeUserPositionRequest.CreateObj(userId, positionId)).Result;
                if (!response.Message.IsSuccess || !response.Message.Body)
                {
                    _logger.LogWarning(logMessage, positionId, userId);

                    errors.Add(errorMessage);
                }

                return response.Message.IsSuccess && response.Message.Body;
            }
            catch (Exception exc)
            {
                _logger.LogWarning(exc, logMessage, positionId, userId);

                errors.Add(errorMessage);
            }

            return false;
        }

        private void ChangeUserRole(Guid roleId, Guid userId, List<string> errors)
        {
            string errorMessage = $"Can't assign role '{roleId}' to the user '{userId}'. Please try again later.";
            const string logMessage = "Can't assign role '{RoleId}' to the user '{UserId}'";

            try
            {
                var response = _rcRole.GetResponse<IOperationResult<bool>>(
                    IChangeUserRoleRequest.CreateObj(
                        roleId,
                        userId,
                        _httpContextAccessor.HttpContext.GetUserId())).Result;
                if (!response.Message.IsSuccess || !response.Message.Body)
                {
                    _logger.LogWarning(logMessage, roleId, userId);

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
            const string logMessage = "Can't assign office '{OfficeId}' to the user '{UserId}'";

            try
            {
                var response = _rcOffice.GetResponse<IOperationResult<bool>>(
                    IChangeUserOfficeRequest.CreateObj(
                        officeId,
                        userId,
                        _httpContextAccessor.HttpContext.GetUserId())).Result;
                if (!response.Message.IsSuccess || !response.Message.Body)
                {
                    _logger.LogWarning(logMessage, officeId, userId);

                    errors.Add(errorMessage);
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, logMessage, officeId, userId);

                errors.Add(errorMessage);
            }
        }

        private void SendEmail(DbUser dbUser, string password, List<string> errors)
        {
            var email = dbUser.Communications.FirstOrDefault(c => c.Type == (int)CommunicationType.Email);

            if (email == null)
            {
                errors.Add("User does not have any linked email.");

                return;
            }

            string errorMessage = $"Can not send email to '{email.Value}'. Email placed in resend queue and will be resent in 1 hour.";

            //TODO: fix add specific template language
            string templateLanguage = "en";
            var senderId = _httpContextAccessor.HttpContext.GetUserId();
            EmailTemplateType templateType = EmailTemplateType.Greeting;
            try
            {
                var templateValues = ISendEmailRequest.CreateTemplateValuesDictionary(
                    userFirstName: dbUser.FirstName,
                    userEmail: email.Value,
                    userId: dbUser.Id.ToString(),
                    userPassword: password);

                var emailRequest = ISendEmailRequest.CreateObj(null, senderId, email.Value, templateLanguage, templateType, templateValues);

                IOperationResult<bool> rcSendEmailResponse = _rcSendEmail
                    .GetResponse<IOperationResult<bool>>(emailRequest, timeout: RequestTimeout.Default)
                    .Result
                    .Message;

                if (!(rcSendEmailResponse.IsSuccess && rcSendEmailResponse.Body))
                {
                    _logger.LogWarning(
                        "Errors while sending email to '{Email}':\n {Errors}", 
                        email.Value,
                        string.Join(Environment.NewLine, rcSendEmailResponse.Errors));

                    errors.Add(errorMessage);
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Can not send email to '{Email}'", email.Value);

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
                var response = _rcImage.GetResponse<IOperationResult<IAddImageResponse>>(
                    IAddImageRequest.CreateObj(
                        avatarRequest.Name,
                        avatarRequest.Content,
                        avatarRequest.Extension,
                        userId)).Result;

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

        public CreateUserCommand(
            ILogger<CreateUserCommand> logger,
            IRequestClient<IAddImageRequest> rcImage,
            IHttpContextAccessor httpContextAccessor,
            IRequestClient<IChangeUserDepartmentRequest> rcDepartment,
            IRequestClient<IChangeUserPositionRequest> rcPosition,
            IRequestClient<IChangeUserRoleRequest> rcRole,
            IRequestClient<IChangeUserOfficeRequest> rcOffice,
            IRequestClient<ISendEmailRequest> rcSendEmail,
            IUserRepository userRepository,
            ICreateUserRequestValidator validator,
            IDbUserMapper mapperUser,
            IAccessValidator accessValidator,
            IGeneratePasswordCommand generatePassword)
        {
            _logger = logger;
            _rcImage = rcImage;
            _rcDepartment = rcDepartment;
            _rcPosition = rcPosition;
            _rcRole = rcRole;
            _rcOffice = rcOffice;
            _rcSendEmail = rcSendEmail;
            _validator = validator;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _mapperUser = mapperUser;
            _accessValidator = accessValidator;
            _generatePassword = generatePassword;
        }

        /// <inheritdoc/>
        public OperationResultResponse<Guid> Execute(CreateUserRequest request)
        {
            if (!(_accessValidator.IsAdmin() ||
                  _accessValidator.HasRights(Rights.AddEditRemoveUsers)))
            {
                throw new ForbiddenException("Not enough rights.");
            }

            _validator.ValidateAndThrowCustom(request);

            OperationResultResponse<Guid> response = new();

            if (_userRepository.IsCommunicationValueExist(request.Communications.Select(x => x.Value).ToList()))
            {
                response.Status = OperationResultStatusType.Conflict;
                response.Errors.Add("Communication value already exist");
                return response;
            }

            Guid? avatarImageId = GetAvatarImageId(request.AvatarImage, response.Errors);

            var dbUser = _mapperUser.Map(request, avatarImageId);

            string password = !string.IsNullOrEmpty(request.Password?.Trim()) ? request.Password.Trim() : _generatePassword.Execute();

            Guid userId = _userRepository.Create(dbUser);
            _userRepository.CreatePending(new DbPendingUser () { UserId = dbUser.Id, Password = password });

            SendEmail(dbUser, password, response.Errors);

            if (!ChangeUserPosition(request.PositionId, userId, response.Errors))
            {
                response.Status = OperationResultStatusType.Failed;
                return response;
            }

            if (request.DepartmentId.HasValue)
            {
                ChangeUserDepartment(request.DepartmentId.Value, userId, response.Errors);
            }

            if (request.RoleId.HasValue)
            {
                ChangeUserRole(request.RoleId.Value, userId, response.Errors);
            }

            ChangeUserOffice(request.OfficeId, userId, response.Errors);

            response.Body = userId;
            response.Status = response.Errors.Any()
                    ? OperationResultStatusType.PartialSuccess
                    : OperationResultStatusType.FullSuccess;
            return response;
        }
    }
}