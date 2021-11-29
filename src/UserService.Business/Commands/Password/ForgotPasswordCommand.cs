using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Message;
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
  // TODO this command does not have any consumers on MessageService side
  /// <inheritdoc/>
  public class ForgotPasswordCommand : IForgotPasswordCommand
  {
    private readonly IGeneratePasswordCommand _generatePassword;
    private readonly ILogger<ForgotPasswordCommand> _logger;
    private readonly IRequestClient<ISendEmailRequest> _rcSendEmail;
    private readonly IOptions<MemoryCacheConfig> _cacheOptions;
    private readonly IUserRepository _repository;
    private readonly IMemoryCache _cache;
    private readonly IResponseCreater _responseCreator;

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
      //TODO: fix add specific template language
      string templateLanguage = "en";
      EmailTemplateType templateType = EmailTemplateType.Warning;
      try
      {
        Dictionary<string, string> templateValues = ISendEmailRequest.CreateTemplateValuesDictionary(
          userFirstName: dbUser.FirstName,
          userId: dbUser.Id.ToString(),
          secret: secret);

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
      IGeneratePasswordCommand generatePassword,
      ILogger<ForgotPasswordCommand> logger,
      IRequestClient<ISendEmailRequest> rcSendEmail,
      IOptions<MemoryCacheConfig> cacheOptions,
      IUserRepository repository,
      IMemoryCache cache,
      IResponseCreater responseCreator)
    {
      _generatePassword = generatePassword;
      _logger = logger;
      _rcSendEmail = rcSendEmail;
      _repository = repository;
      _cacheOptions = cacheOptions;
      _cache = cache;
      _responseCreator = responseCreator;
    }

    /// <inheritdoc/>
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
