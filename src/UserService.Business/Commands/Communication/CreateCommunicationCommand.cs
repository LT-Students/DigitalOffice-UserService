using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
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
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Configurations;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Communication;
using LT.DigitalOffice.UserService.Validation.Communication.Interfaces;
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
  public class CreateCommunicationCommand : ICreateCommunicationCommand
  {
    private readonly ICreateCommunicationRequestValidator _validator;
    private readonly IDbUserCommunicationMapper _mapper;
    private readonly IUserCommunicationRepository _repository;
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IResponseCreator _responseCreator;
    private readonly IMemoryCache _cache;
    private readonly IOptions<MemoryCacheConfig> _cacheOptions;
    private readonly ITextTemplateParser _parser;
    private readonly ITextTemplateService _textTemplateService;
    private readonly IEmailService _emailService;

    private async Task NotifyAsync(
      DbUserCommunication dbUserCommunication,
      string secret,
      string locale,
      List<string> errors)
    {
      IGetTextTemplateResponse textTemplate = await _textTemplateService
        .GetAsync(TemplateType.ConfirmСommunication, locale, errors);

      if (textTemplate is null)
      {
        return;
      }

      string parsedText = _parser.Parse(
        new Dictionary<string, string> { { "Secret", secret } },
        _parser.ParseModel<DbUserCommunication>(dbUserCommunication, textTemplate.Text));

      await _emailService.SendAsync(dbUserCommunication.Value, textTemplate.Subject, parsedText, errors);
    }

    public CreateCommunicationCommand(
      ICreateCommunicationRequestValidator validator,
      IDbUserCommunicationMapper mapper,
      IUserCommunicationRepository repository,
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IResponseCreator responseCreator,
      IMemoryCache cache,
      IOptions<MemoryCacheConfig> cacheOptions,
      ITextTemplateParser parser,
      ITextTemplateService textTemplateService,
      IEmailService emailService)
    {
      _validator = validator;
      _mapper = mapper;
      _repository = repository;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _responseCreator = responseCreator;
      _cache = cache;
      _cacheOptions = cacheOptions;
      _textTemplateService = textTemplateService;
      _parser = parser;
      _emailService = emailService;
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

      DbUserCommunication dbUserCommunication = _mapper.Map(request);

      OperationResultResponse<Guid?> response = new(
        await _repository.CreateAsync(dbUserCommunication));

      if (response.Body is null)
      {
        return _responseCreator.CreateFailureResponse<Guid?>(HttpStatusCode.BadRequest);
      }

      if (request.Type == CommunicationType.Email)
      {
        string secret = Guid.NewGuid().ToString();

        _cache.Set(dbUserCommunication.Id, secret, TimeSpan.FromMinutes(_cacheOptions.Value.CacheLiveInMinutes));

        await NotifyAsync(dbUserCommunication, secret, "ru", response.Errors);
      }

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      return response;
    }
  }
}
