using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Helpers.TextHandlers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Email;
using LT.DigitalOffice.Models.Broker.Requests.TextTemplate;
using LT.DigitalOffice.Models.Broker.Responses.TextTemplate;
using LT.DigitalOffice.UserService.Business.Commands.Password.Interfaces;
using LT.DigitalOffice.UserService.Business.Commands.Pending.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Pending
{
  public class ResendInvitationCommand : IResendInvitationCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IPendingUserRepository _repository;
    private readonly IRequestClient<IGetTextTemplateRequest> _rcGetTextTemplate;
    private readonly IRequestClient<ISendEmailRequest> _rcSendEmail;
    private readonly ILogger<ResendInvitationCommand> _logger;
    private readonly IGeneratePasswordCommand _generatePassword;
    private readonly ITextTemplateParser _parser;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IResponseCreator _responseCreator;

    private async Task SendEmailAsync(DbPendingUser dbPendingUser, List<string> errors)
    {
      IGetTextTemplateResponse textTemplate =
        await RequestHandler.ProcessRequest<IGetTextTemplateRequest, IGetTextTemplateResponse>(
          _rcGetTextTemplate,
          IGetTextTemplateRequest.CreateObj(
            endpointId: Guid.Empty, //TO DO get guid from cache
            templateType: TemplateType.Greeting,
            locale: "en"),
          errors,
          _logger);

      if (textTemplate is null)
      {
        _logger.LogError(
          "Not found text template to send email to user id '{UserId}'",
          dbPendingUser.UserId);

        return;
      }

      string email = dbPendingUser.User.Communications
        .FirstOrDefault(c => c.Id == dbPendingUser.CommunicationId)
        .Value;

      string parsedText = _parser.Parse(
        new Dictionary<string, string> { { "Password", dbPendingUser.Password } },
        _parser.ParseModel<DbUser>(dbPendingUser.User, textTemplate.Text));

      if (!await RequestHandler.ProcessRequest<ISendEmailRequest, bool>(
        _rcSendEmail,
        ISendEmailRequest.CreateObj(
          email,
          textTemplate.Subject,
          parsedText,
          _httpContextAccessor.HttpContext.GetUserId()),
        errors,
        _logger))
      {
        _logger.LogError(
          "Invitation letter not sent to email '{Email}'",
          email);

        errors.Add($"Can not send email to '{email}'. Email placed in resend queue and will be resent in 1 hour.");
      }
    }

    public ResendInvitationCommand(
      IAccessValidator accessValidator,
      IPendingUserRepository repository,
      IRequestClient<IGetTextTemplateRequest> rcGetTextTemplate,
      IRequestClient<ISendEmailRequest> rcSendEmail,
      ILogger<ResendInvitationCommand> logger,
      IGeneratePasswordCommand generatePassword,
      ITextTemplateParser parser,
      IHttpContextAccessor httpContextAccessor,
      IResponseCreator responseCreator)
    {
      _accessValidator = accessValidator;
      _repository = repository;
      _rcGetTextTemplate = rcGetTextTemplate;
      _rcSendEmail = rcSendEmail;
      _logger = logger;
      _generatePassword = generatePassword;
      _parser = parser;
      _httpContextAccessor = httpContextAccessor;
      _responseCreator = responseCreator;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid userId, Guid communicationId)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers))
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      DbPendingUser dbPendingUser = await _repository.GetAsync(userId, true);

      if (dbPendingUser is null)
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.NotFound);
      }

      if (dbPendingUser.CommunicationId != communicationId)
      {
        if (dbPendingUser.User.Communications.FirstOrDefault(c => c.Id == communicationId) is not null)
        {
          dbPendingUser.CommunicationId = communicationId;
        }
        else
        {
          return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest);
        }
      }

      dbPendingUser.Password = _generatePassword.Execute();

      OperationResultResponse<bool> response = new();

      await _repository.UpdateAsync(dbPendingUser);

      await SendEmailAsync(dbPendingUser, response.Errors);

      response.Status = response.Errors.Any()
        ? Kernel.Enums.OperationResultStatusType.Failed
        : Kernel.Enums.OperationResultStatusType.PartialSuccess;

      response.Body = true;

      return response;
    }
  }
}
