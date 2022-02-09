using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.Constants;
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
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Configurations;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Communication;
using LT.DigitalOffice.UserService.Validation.Communication.Interfaces;
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

namespace LT.DigitalOffice.UserService.Business.Commands.Communication
{
  public class CreateCommunicationCommand : ICreateCommunicationCommand
  {
    private readonly ICreateCommunicationRequestValidator _validator;
    private readonly IDbUserCommunicationMapper _mapper;
    private readonly ICommunicationRepository _repository;
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IResponseCreator _responseCreator;
    private readonly IMemoryCache _cache;
    private readonly IOptions<MemoryCacheConfig> _cacheOptions;
    private readonly ILogger<CreateCommunicationCommand> _logger;
    private readonly IRequestClient<IGetTextTemplateRequest> _rcGetTextTemplate;
    private readonly IRequestClient<ISendEmailRequest> _rcSendEmail;
    private readonly ITextTemplateParser _parser;

    private async Task SendEmailAsync(DbUserCommunication dbUserCommunication, string secret, List<string> errors)
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
        new Dictionary<string, string> { { "Secret", secret } },
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

    public CreateCommunicationCommand(
      ICreateCommunicationRequestValidator validator,
      IDbUserCommunicationMapper mapper,
      ICommunicationRepository repository,
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IResponseCreator responseCreator,
      IMemoryCache cache,
      IOptions<MemoryCacheConfig> cacheOptions,
      ILogger<CreateCommunicationCommand> logger,
      IRequestClient<IGetTextTemplateRequest> rcGetTextTemplate,
      IRequestClient<ISendEmailRequest> rcSendEmail,
      ITextTemplateParser parser)
    {
      _validator = validator;
      _mapper = mapper;
      _repository = repository;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _responseCreator = responseCreator;
      _cache = cache;
      _cacheOptions = cacheOptions;
      _logger = logger;
      _rcGetTextTemplate = rcGetTextTemplate;
      _rcSendEmail = rcSendEmail;
      _parser = parser;
    }

    public async Task<OperationResultResponse<Guid?>> ExecuteAsync(CreateCommunicationRequest request)
    {
      if ((request.UserId != _httpContextAccessor.HttpContext.GetUserId()) &&
        !await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers))
      {
        return _responseCreator.CreateFailureResponse<Guid?>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator.ValidateAsync(request);

      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<Guid?>(HttpStatusCode.BadRequest,
          validationResult.Errors.Select(vf => vf.ErrorMessage).ToList());
      }

      OperationResultResponse<Guid?> response = new();

      DbUserCommunication dbUserCommunication = _mapper.Map(request);

      response.Body = await _repository.CreateAsync(dbUserCommunication);

      if (response.Body is null)
      {
        return _responseCreator.CreateFailureResponse<Guid?>(HttpStatusCode.BadRequest);
      }

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      if (request.Type == Models.Dto.Enums.CommunicationType.Email)
      {
        string secret = Guid.NewGuid().ToString();

        _cache.Set(dbUserCommunication.Id, secret, TimeSpan.FromMinutes(_cacheOptions.Value.CacheLiveInMinutes));

        await SendEmailAsync(dbUserCommunication, secret, response.Errors);
      }

      response.Status = response.Errors.Any()
        ? OperationResultStatusType.Failed
        : OperationResultStatusType.PartialSuccess;

      return response;
    }
  }
}
