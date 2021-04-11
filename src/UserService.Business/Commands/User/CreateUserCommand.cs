using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.MessageService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Responses;
using LT.DigitalOffice.UserService.Validation.User.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        private void ChangeUserDepartment(Guid departmentId, Guid userId, List<string> errors)
        {
            // TODO add user department
        }

        private void ChangeUserPosition(Guid positionId, Guid userId, List<string> errors)
        {
            // TODO add user position
        }

        // TODO add resend logic
        private void SendEmail(DbUser dbUser, string password, List<string> errors)
        {
            var email = dbUser.Communications.FirstOrDefault(c => c.Type == (int)CommunicationType.Email);

            if (email == null)
            {
                errors.Add("User does not have any linked email.");

                return;
            }

            string errorMessage = $"Can not send email to '{email.Value}'. Email placed in resend queue and will be resended in 1 hour.";

            string languageTemplate = "en";
            Guid senderId = _httpContextAccessor.HttpContext.GetUserId();
            try
            {
                string link = $"http://localhost:4200/auth/firstlogin?userId={dbUser.Id}";

                StringBuilder sb = new();
                sb.AppendLine($"Hello, {dbUser.FirstName}!!!");
                sb.AppendLine();
                sb.AppendLine("You receive this message because you was invited to join Digital Office community.");
                sb.AppendLine("If you sure that it is not for you just ignore this message.");
                sb.AppendLine($"In other case please follow this link: {link}");
                sb.AppendLine($"Your password: {password}");
                sb.AppendLine();
                sb.AppendLine("Best Regards,");
                sb.AppendLine("Digital Office team.");

                var templateTags = _rcGetTemplateTags.GetResponse<IOperationResult<IGetEmailTemplateTagsResponse>>(
                    IGetEmailTemplateTagsRequest.CreateObj(
                        languageTemplate,
                        EmailTemplateType.Greeting)).Result.Message;

                var templateValues = templateTags.Body.CreateDictionaryTemplate(
                    dbUser.FirstName, email.Value, dbUser.Id.ToString(), password, null);

                // TODO add email template ID
                IOperationResult<bool> response = _rcSendEmail.GetResponse<IOperationResult<bool>>(
                    ISendEmailRequest.CreateObj(
                        templateTags.Body.TemplateId,
                        senderId,
                        email.Value,
                        languageTemplate,
                        templateValues
                       )).Result.Message;

                if (!response.IsSuccess)
                {
                    _logger.LogWarning(
                        $"Errors while sending email to '{email.Value}':{Environment.NewLine}{string.Join('\n', response.Errors)}.");

                    errors.Add(errorMessage);
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, errorMessage);

                errors.Add(errorMessage);
            }
        }

        private Guid? GetAvatarImageId(CreateUserRequest request, List<string> errors)
        {
            Guid? avatarImageId = null;

            if (!string.IsNullOrEmpty(request.AvatarImage))
            {
                string errorMessage = $"Can not add avatar image to user. Please try again later.";

                try
                {
                    Response<IOperationResult<Guid>> response = _rcImage.GetResponse<IOperationResult<Guid>>(
                        IAddImageRequest.CreateObj(request.AvatarImage),
                        default,
                        timeout: RequestTimeout.After(ms: 500)).Result;

                    if (!response.Message.IsSuccess)
                    {
                        _logger.LogWarning($"Can not add avatar image. Reason: '{string.Join(',', response.Message.Errors)}'");

                        errors.Add(errorMessage);
                    }
                    else
                    {
                        avatarImageId = response.Message.Body;
                    }
                }
                catch (Exception exc)
                {
                    _logger.LogError(exc, errorMessage);

                    errors.Add(errorMessage);
                }
            }

            return avatarImageId;
        }

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

            Guid? avatarImageId = GetAvatarImageId(request, errors);

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