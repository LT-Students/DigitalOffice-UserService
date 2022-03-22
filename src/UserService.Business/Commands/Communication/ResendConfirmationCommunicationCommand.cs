using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Helpers.TextHandlers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Responses.TextTemplate;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using LT.DigitalOffice.UserService.Business.Commands.Communication.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Configurations;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Communication
{
  public class ResendConfirmationCommunicationCommand : IResendConfirmationCommunicationCommand
  {
    private readonly IUserCommunicationRepository _repository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IResponseCreator _responseCreator;
    private readonly IMemoryCache _cache;
    private readonly IOptions<MemoryCacheConfig> _cacheOptions;
    private readonly ITextTemplateService _textTemplateService;
    private readonly IEmailService _emailService;
    private readonly ITextTemplateParser _parser;

    private async Task NotifyAsync(DbUserCommunication dbUserCommunication, string secret, string locale, List<string> errors)
    {
      IGetTextTemplateResponse textTemplate = await _textTemplateService.GetAsync(
        templateType: TemplateType.ConfirmСommunication,
        locale: locale,
        errors: errors);

      if (textTemplate is null)
      {
        return;
      }

      string parsedText = _parser.Parse(
        new Dictionary<string, string> { { "Secret", secret } },
        _parser.ParseModel<DbUserCommunication>(dbUserCommunication, textTemplate.Text));

      await _emailService.SendAsync(
        dbUserCommunication.Value,
        textTemplate.Subject,
        parsedText,
        errors);
    }

    public ResendConfirmationCommunicationCommand(
      IUserCommunicationRepository repository,
      IHttpContextAccessor httpContextAccessor,
      IResponseCreator responseCreator,
      IMemoryCache cache,
      IOptions<MemoryCacheConfig> cacheOptions,
      ITextTemplateService textTemplateService,
      IEmailService emailService,
      ITextTemplateParser parser)
    {
      _repository = repository;
      _httpContextAccessor = httpContextAccessor;
      _responseCreator = responseCreator;
      _cache = cache;
      _cacheOptions = cacheOptions;
      _textTemplateService = textTemplateService;
      _emailService = emailService;
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

      _cache.Set(communicationId, secret, TimeSpan.FromMinutes(_cacheOptions.Value.CacheLiveInMinutes));

      await NotifyAsync(dbUserCommunication, secret, "ru", response.Errors);

      response.Body = response.Errors.Any() ? false : true;
      response.Status = response.Errors.Any()
        ? OperationResultStatusType.Failed
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
