using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Helpers.TextHandlers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Email;
using LT.DigitalOffice.Models.Broker.Requests.TextTemplate;
using LT.DigitalOffice.Models.Broker.Responses.TextTemplate;
using LT.DigitalOffice.UserService.Business.Commands.Password.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Configurations;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters;
using MassTransit;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Password
{
  public class ForgotPasswordCommand : IForgotPasswordCommand
  {
    private readonly IGeneratePasswordCommand _generatePassword;
    private readonly ILogger<ForgotPasswordCommand> _logger;
    private readonly IRequestClient<IGetTextTemplateRequest> _rcGetTextTemplate;
    private readonly IRequestClient<ISendEmailRequest> _rcSendEmail;
    private readonly IOptions<MemoryCacheConfig> _cacheOptions;
    private readonly IUserRepository _repository;
    private readonly IMemoryCache _cache;
    private readonly ITextTemplateParser _parser;
    private readonly IResponseCreator _responseCreator;

    private string SetGuidInCache(Guid userId)
    {
      string secret = _generatePassword.Execute();

      _cache.Set(secret, userId, new MemoryCacheEntryOptions
      {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheOptions.Value.CacheLiveInMinutes)
      });

      return secret;
    }

    private async Task<bool> SendEmailAsync(
      DbUser dbUser,
      string email,
      string secret,
      List<string> errors)
    {
      try
      {
        Response<IOperationResult<IGetTextTemplateResponse>> textTemplateResponse =
          await _rcGetTextTemplate.GetResponse<IOperationResult<IGetTextTemplateResponse>>(
            IGetTextTemplateRequest.CreateObj(
              endpointId: Guid.Empty,
              //ToDo add template types
              templateType: TemplateType.Notification,
              locale: "en"));

        if (!textTemplateResponse.Message.IsSuccess || textTemplateResponse.Message.Body is null)
        {
          _logger.LogWarning(
            "Errors while getting text template':\n {Errors}",
            string.Join(Environment.NewLine, textTemplateResponse.Message.Errors));

          errors.Add("Email template not found");
          return false;
        }

        string parsedText = _parser.Parse(
          new Dictionary<string, string> { { "Password", secret } },
          _parser.ParseModel<DbUser>(dbUser, textTemplateResponse.Message.Body.Text));

        Response<IOperationResult<bool>> sendEmailResponse =
          await _rcSendEmail.GetResponse<IOperationResult<bool>>(
            ISendEmailRequest.CreateObj(
              //ToDo fix model
              Guid.Empty,
              email,
              textTemplateResponse.Message.Body.Subject,
              parsedText));

        if (sendEmailResponse.Message.IsSuccess || sendEmailResponse.Message.Body)
        {
          return true;
        }

        _logger.LogWarning(
          "Errors while sending email to '{Email}':\n {Errors}",
          email,
          string.Join(Environment.NewLine, sendEmailResponse.Message.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, "Can not send email to '{Email}'", email);
      }
      errors.Add($"Can not send email to '{email}'. Email placed in resend queue and will be resent in 1 hour.");

      return false;
    }

    public ForgotPasswordCommand(
      IGeneratePasswordCommand generatePassword,
      ILogger<ForgotPasswordCommand> logger,
      IRequestClient<IGetTextTemplateRequest> rcGetTextTemplate,
      IRequestClient<ISendEmailRequest> rcSendEmail,
      IOptions<MemoryCacheConfig> cacheOptions,
      IUserRepository repository,
      IMemoryCache cache,
      ITextTemplateParser parser,
      IResponseCreator responseCreator)
    {
      _generatePassword = generatePassword;
      _logger = logger;
      _rcGetTextTemplate = rcGetTextTemplate;
      _rcSendEmail = rcSendEmail;
      _repository = repository;
      _cacheOptions = cacheOptions;
      _cache = cache;
      _parser = parser;
      _responseCreator = responseCreator;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(string userEmail)
    {
      DbUser dbUser = string.IsNullOrEmpty(userEmail) ?
        null :
        await _repository.GetAsync(new GetUserFilter() { Email = userEmail, IncludeCommunications = true });

      if (dbUser is null)
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.NotFound);
      }

      string secret = SetGuidInCache(dbUser.Id);

      OperationResultResponse<bool> response = new();

      response.Body = await SendEmailAsync(dbUser, userEmail, secret, response.Errors);
      response.Status = response.Body ?
        OperationResultStatusType.FullSuccess :
        OperationResultStatusType.Failed;

      return response;
    }
  }
}
