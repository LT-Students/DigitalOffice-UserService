﻿using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Message;
using LT.DigitalOffice.UserService.Business.Commands.Password.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Configurations;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters;
using LT.DigitalOffice.UserService.Validation.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Business.Commands.Password
{
    // TODO this command does not have any consumers on MessageService side
    /// <inheritdoc/>
    public class ForgotPasswordCommand : IForgotPasswordCommand
    {
        private readonly ILogger<ForgotPasswordCommand> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRequestClient<ISendEmailRequest> _rcSendEmail;
        private readonly IOptions<CacheConfig> _cacheOptions;
        private readonly IEmailValidator _validator;
        private readonly IUserRepository _repository;
        private readonly IMemoryCache _cache;

        private Guid SetGuidInCache(Guid userId)
        {
            var secret = Guid.NewGuid();

            _cache.Set(secret, userId, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheOptions.Value.CacheLiveInMinutes)
            });

            return secret;
        }

        private bool SendEmail(
            DbUser dbUser,
            string email,
            Guid secret,
            List<string> errors)
        {
            string errorMessage = $"Can not send email to '{email}'. Please try again latter.";

            //TODO: fix add specific template language
            string templateLanguage = "en";
            Guid senderId = _httpContextAccessor.HttpContext.GetUserId();
            EmailTemplateType templateType = EmailTemplateType.Warning;
            try
            {
                var templateValues = ISendEmailRequest.CreateTemplateValuesDictionary(
                    userFirstName: dbUser.FirstName,
                    userId: dbUser.Id.ToString(),
                    secret: secret.ToString());

                var emailRequest = ISendEmailRequest.CreateObj(null, senderId, email, templateLanguage, templateType, templateValues);

                IOperationResult<bool> response = _rcSendEmail
                    .GetResponse<IOperationResult<bool>>(emailRequest, timeout: RequestTimeout.Default)
                    .Result
                    .Message;

                if (!response.IsSuccess)
                {
                    _logger.LogWarning(
                        $"Errors while sending email to '{email}':{Environment.NewLine}{string.Join('\n', response.Errors)}.");

                    errors.Add(errorMessage);

                    return false;
                }

                return true;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, errorMessage);

                errors.Add(errorMessage);

                return false;
            }
        }

        public ForgotPasswordCommand(
            ILogger<ForgotPasswordCommand> logger,
            IRequestClient<ISendEmailRequest> rcSendEmail,
            IOptions<CacheConfig> cacheOptions,
            IHttpContextAccessor httpContextAccessor,
            IEmailValidator validator,
            IUserRepository repository,
            IMemoryCache cache)
        {
            _logger = logger;
            _rcSendEmail = rcSendEmail;
            _httpContextAccessor = httpContextAccessor;
            _repository = repository;
            _validator = validator;
            _cacheOptions = cacheOptions;
            _cache = cache;
        }

        /// <inheritdoc/>
        public OperationResultResponse<bool> Execute(string userEmail)
        {
            _validator.ValidateAndThrowCustom(userEmail);

            var filter = new GetUserFilter
            {
                Email = userEmail,
                IncludeCommunications = true
            };

            var dbUser = _repository.Get(filter);
            if (dbUser == null)
            {
                throw new NotFoundException($"User with email '{userEmail}' was not found.");
            }

            var secret = SetGuidInCache(dbUser.Id);

            List<string> errors = new();

            bool result = SendEmail(dbUser, userEmail, secret, errors);

            return new OperationResultResponse<bool>
            {
                Status = errors.Any()
                    ? OperationResultStatusType.Failed
                    : OperationResultStatusType.FullSuccess,
                Body = result,
                Errors = errors
            };
        }
    }
}
