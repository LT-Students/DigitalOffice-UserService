using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Message;
using LT.DigitalOffice.UserService.Business.Commands.Password.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Configurations;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters;
using LT.DigitalOffice.UserService.Validation.Email.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Password
{
  // TODO this command does not have any consumers on MessageService side
  /// <inheritdoc/>
  public class ForgotPasswordCommand : IForgotPasswordCommand
  {
    private readonly ILogger<ForgotPasswordCommand> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRequestClient<ISendEmailRequest> _rcSendEmail;
    private readonly IOptions<MemoryCacheConfig> _cacheOptions;
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

    private async Task<bool> SendEmailAsync(
      DbUser dbUser,
      string email,
      Guid secret,
      List<string> errors)
    {
      //TODO: fix add specific template language
      string templateLanguage = "en";
      EmailTemplateType templateType = EmailTemplateType.Warning;
      try
      {
        Dictionary<string, string> templateValues = ISendEmailRequest.CreateTemplateValuesDictionary(
          userFirstName: dbUser.FirstName,
          userId: dbUser.Id.ToString(),
          secret: secret.ToString());

        Response<IOperationResult<bool>> response =
          await _rcSendEmail
            .GetResponse<IOperationResult<bool>>(ISendEmailRequest.CreateObj(
              null,
              dbUser.Id,
              email,
              templateLanguage,
              templateType,
              templateValues));

        if (response.Message.IsSuccess)
        {
          return response.Message.Body;
        }

        _logger.LogWarning(
          "Errors while sending email to '{Email}':\n{Errors}",
          email,
          string.Join('\n', response.Message.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, "Can not send email to '{Email}'", email);
      }

      errors.Add("Can not send email. Please try again latter.");

      return false;
    }

    public ForgotPasswordCommand(
      ILogger<ForgotPasswordCommand> logger,
      IRequestClient<ISendEmailRequest> rcSendEmail,
      IOptions<MemoryCacheConfig> cacheOptions,
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
    public async Task<OperationResultResponse<bool>> ExecuteAsync(string userEmail)
    {
      if (!_validator.ValidateCustom(userEmail, out List<string> errors))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        return new()
        {
          Status = OperationResultStatusType.Failed,
          Errors = errors
        };
      }

      GetUserFilter filter = new()
      {
        Email = userEmail,
        IncludeCommunications = true
      };

      DbUser dbUser = await _repository.GetAsync(filter);

      if (dbUser == null)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;

        return new()
        {
          Status = OperationResultStatusType.Failed
        };
      }

      Guid secret = SetGuidInCache(dbUser.Id);

      OperationResultResponse<bool> response = new();

      response.Body = await SendEmailAsync(dbUser, userEmail, secret, response.Errors);
      response.Status = errors.Any()
        ? OperationResultStatusType.Failed
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
