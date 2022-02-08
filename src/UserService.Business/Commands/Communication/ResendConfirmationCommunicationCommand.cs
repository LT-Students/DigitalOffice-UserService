using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Helpers.TextHandlers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Email;
using LT.DigitalOffice.Models.Broker.Requests.TextTemplate;
using LT.DigitalOffice.Models.Broker.Responses.TextTemplate;
using LT.DigitalOffice.UserService.Business.Commands.Communication.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Communication
{
  public class ResendConfirmationCommunicationCommand : IResendConfirmationCommunicationCommand
  {
    private readonly ICommunicationRepository _repository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IResponseCreator _responseCreator;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ResendConfirmationCommunicationCommand> _logger;
    private readonly IRequestClient<IGetTextTemplateRequest> _rcGetTextTemplate;
    private readonly IRequestClient<ISendEmailRequest> _rcSendEmail;
    private readonly ITextTemplateParser _parser;

    private async Task SendEmailAsync(DbUserCommunication dbUserCommunication, string password, List<string> errors)
    {

      IGetTextTemplateResponse textTemplate =
        await RequestHandler.ProcessRequest<IGetTextTemplateRequest, IGetTextTemplateResponse>(
          _rcGetTextTemplate,
          IGetTextTemplateRequest.CreateObj(
            endpointId: Guid.Empty, //TO DO get guid from cache
            templateType: TemplateType.ConfirmСommunication,
            locale: "en"),
          errors,
          _logger);

      if (textTemplate is null)
      {
        _logger.LogError(
          "Not found text template to send email to user id '{UserId}'",
          dbUserCommunication.UserId);

        return;
      }

      string parsedText = _parser.Parse(
        new Dictionary<string, string> { { "Password", password } },
        _parser.ParseModel<DbUserCommunication>(dbUserCommunication, textTemplate.Text));

      if (!await RequestHandler.ProcessRequest<ISendEmailRequest, bool>(
        _rcSendEmail,
        ISendEmailRequest.CreateObj(
          dbUserCommunication.Value,
          textTemplate.Subject,
          parsedText,
          _httpContextAccessor.HttpContext.GetUserId()),
        errors,
        _logger))
      {
        _logger.LogError(
          "Invitation letter not sent to email '{Email}'",
          dbUserCommunication.Value);

        errors.Add($"Can not send email to '{dbUserCommunication.Value}'. Email placed in resend queue and will be resent in 1 hour.");
      }
    }

    public ResendConfirmationCommunicationCommand(
      ICommunicationRepository repository,
      IHttpContextAccessor httpContextAccessor,
      IResponseCreator responseCreator,
      IMemoryCache cache,
      ILogger<ResendConfirmationCommunicationCommand> logger,
      IRequestClient<IGetTextTemplateRequest> rcGetTextTemplate,
      IRequestClient<ISendEmailRequest> rcSendEmail,
      ITextTemplateParser parser)
    {
      _repository = repository;
      _httpContextAccessor = httpContextAccessor;
      _responseCreator = responseCreator;
      _cache = cache;
      _logger = logger;
      _rcGetTextTemplate = rcGetTextTemplate;
      _rcSendEmail = rcSendEmail;
      _parser = parser;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid communicationId)
    {
      DbUserCommunication dbUserCommunication = await _repository.GetAsync(communicationId);

      if (dbUserCommunication is null)
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.NotFound);
      }

      if (_httpContextAccessor.HttpContext.GetUserId() != dbUserCommunication.UserId)
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      if (dbUserCommunication.Type != (int)CommunicationType.Email)
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest);
      }

      OperationResultResponse<bool> response = new();

      string secret = Guid.NewGuid().ToString();

      _cache.Set(communicationId, secret);

      await SendEmailAsync(dbUserCommunication, secret, response.Errors);

      response.Body = response.Errors.Any() ? false : true;
      response.Status = response.Errors.Any()
        ? OperationResultStatusType.Failed
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
