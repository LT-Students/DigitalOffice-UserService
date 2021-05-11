using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.MessageService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Business.Helpers.Email;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Models.Dto.Responses;
using LT.DigitalOffice.UserService.Validation.User.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Business
{
    /// <inheritdoc/>
    public class CreateUserCommand : ICreateUserCommand
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CreateUserCommand> _logger;
        private readonly IRequestClient<IAddImageRequest> _rcImage;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRequestClient<IGetEmailTemplateTagsRequest> _rcGetTemplateTags;
        private readonly IRequestClient<IChangeUserDepartmentRequest> _rcDepartment;
        private readonly IRequestClient<IChangeUserPositionRequest> _rcPosition;
        private readonly IRequestClient<ISendEmailRequest> _rcSendEmail;
        private readonly ICreateUserRequestValidator _validator;
        private readonly IDbUserMapper _mapperUser;
        private readonly IAccessValidator _accessValidator;

        #region private methods
        private void ChangeUserDepartment(Guid departmentId, Guid userId, List<string> errors)
        {
            // TODO add user department
        }

        private void ChangeUserPosition(Guid positionId, Guid userId, List<string> errors)
        {
            // TODO add user position
        }

        private void SendEmail(DbUser dbUser, string password, List<string> errors)
        {
            var email = dbUser.Communications.FirstOrDefault(c => c.Type == (int)CommunicationType.Email);

            if (email == null)
            {
                errors.Add("User does not have any linked email.");

                return;
            }

            string errorMessage = $"Can not send email to '{email.Value}'. Email placed in resend queue and will be resended in 1 hour.";

            object emailRequest = null;

            //TODO: fix add specific template language
            string templateLanguage = "en";
            Guid senderId = _httpContextAccessor.HttpContext.GetUserId();
            EmailTemplateType templateType = EmailTemplateType.Warning;
            try
            {
                var rcGetTemplateTagsResponse = _rcGetTemplateTags.GetResponse<IOperationResult<IGetEmailTemplateTagsResponse>>(
                    IGetEmailTemplateTagsRequest.CreateObj(
                        templateLanguage,
                        templateType)).Result.Message;

                var templateValues = rcGetTemplateTagsResponse.Body.CreateDictionaryTemplate(
                    dbUser.FirstName, email.Value, dbUser.Id.ToString(), password, null);

                if (!rcGetTemplateTagsResponse.IsSuccess)
                {
                    _logger.LogWarning(
                        $"Errors while get email template tags of type:'{templateType}':" +
                        $"{Environment.NewLine}{string.Join('\n', rcGetTemplateTagsResponse.Errors)}.");

                    errors.Add(errorMessage);
                }

                emailRequest = ISendEmailRequest.CreateObj(
                        rcGetTemplateTagsResponse.Body.TemplateId,
                        senderId,
                        email.Value,
                        templateLanguage,
                        templateValues);
                IOperationResult<bool> rcSendEmailResponse = _rcSendEmail
                    .GetResponse<IOperationResult<bool>>(emailRequest)
                    .Result
                    .Message;

                if (!rcSendEmailResponse.IsSuccess)
                {
                    _logger.LogWarning(
                        $"Errors while sending email to '{email.Value}':{Environment.NewLine}{string.Join('\n', rcSendEmailResponse.Errors)}.");

                    errors.Add(errorMessage);

                    EmailResender.AddToQueue(emailRequest);
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, errorMessage);

                errors.Add(errorMessage);

                if (emailRequest != null)
                {
                    EmailResender.AddToQueue(emailRequest);
                }
            }
        }

        private Guid? GetAvatarImageId(AddImageRequest avatarRequest, List<string> errors)
        {
            Guid? avatarImageId = null;

            if (avatarRequest is null)
            {
                return avatarImageId;
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

        public CreateUserCommand(
            ILogger<CreateUserCommand> logger,
            IRequestClient<IAddImageRequest> rcImage,
            IHttpContextAccessor httpContextAccessor,
            IRequestClient<IGetEmailTemplateTagsRequest> rcGetTemplateTags,
            IRequestClient<IChangeUserDepartmentRequest> rcDepartment,
            IRequestClient<IChangeUserPositionRequest> rcPosition,
            IRequestClient<ISendEmailRequest> rcSendEmail,
            IUserRepository userRepository,
            ICreateUserRequestValidator validator,
            IDbUserMapper mapperUser,
            IAccessValidator accessValidator)
        {
            _logger = logger;
            _rcImage = rcImage;
            _rcDepartment = rcDepartment;
            _rcPosition = rcPosition;
            _rcSendEmail = rcSendEmail;
            _validator = validator;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _mapperUser = mapperUser;
            _accessValidator = accessValidator;
            _rcGetTemplateTags = rcGetTemplateTags;
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

            List<string> errors = new();

            Guid? avatarImageId = GetAvatarImageId(request.AvatarImage, errors);

            var dbUser = _mapperUser.Map(request, avatarImageId);

            Guid userId = _userRepository.Create(dbUser, request.Password);

            SendEmail(dbUser, request.Password, errors);

            if (request.DepartmentId.HasValue)
            {
                ChangeUserDepartment(request.DepartmentId.Value, userId, errors);
            }

            if (request.PositionId.HasValue)
            {
                ChangeUserPosition(request.PositionId.Value, userId, errors);
            }

            return new OperationResultResponse<Guid>
            {
                Body = userId,
                Status = errors.Any()
                    ? OperationResultStatusType.PartialSuccess
                    : OperationResultStatusType.FullSuccess,
                Errors = errors
            };
        }
    }
}