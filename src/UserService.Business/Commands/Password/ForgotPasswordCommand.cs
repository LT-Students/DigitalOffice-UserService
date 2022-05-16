using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Helpers.TextHandlers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Responses.TextTemplate;
using LT.DigitalOffice.UserService.Broker.Helpers.Login;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using LT.DigitalOffice.UserService.Business.Commands.Password.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Configurations;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Password
{
  public class ForgotPasswordCommand : IForgotPasswordCommand
  {
    private readonly IGeneratePasswordCommand _generatePassword;
    private readonly IOptions<MemoryCacheConfig> _cacheOptions;
    private readonly IUserRepository _repository;
    private readonly IMemoryCache _cache;
    private readonly ITextTemplateParser _parser;
    private readonly IResponseCreator _responseCreator;
    private readonly ITextTemplateService _textTemplateService;
    private readonly IEmailService _emailService;

    private GetUserFilter CreateFilter(string LoginData)
    {
      GetUserFilter filter = new();

      if (LoginData.IsEmail())
      {
        filter.Email = LoginData;
      }
      else
      {
        filter.Login = LoginData;
      }

      filter.IncludeCommunications = true;

      return filter;
    }

    private string SetGuidInCache(Guid userId)
    {
      string secret = _generatePassword.Execute();

      _cache.Set(secret, userId, new MemoryCacheEntryOptions
      {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheOptions.Value.CacheLiveInMinutes)
      });

      return secret;
    }

    private async Task NotifyAsync(
      DbUser dbUser,
      string email,
      string secret,
      string locale,
      List<string> errors)
    {
      IGetTextTemplateResponse textTemplate = await _textTemplateService
        .GetAsync(TemplateType.PasswordRecovery, locale, errors);

      if (textTemplate is null)
      {
        return;
      }

      string parsedText = _parser.Parse(
        new Dictionary<string, string> { { "Password", secret } },
        _parser.ParseModel<DbUser>(dbUser, textTemplate.Text));

      await _emailService.SendAsync(email, textTemplate.Subject, parsedText, errors);
    }

    public ForgotPasswordCommand(
      IGeneratePasswordCommand generatePassword,
      IOptions<MemoryCacheConfig> cacheOptions,
      IUserRepository repository,
      IMemoryCache cache,
      ITextTemplateParser parser,
      IResponseCreator responseCreator,
      ITextTemplateService textTemplateService,
      IEmailService emailService)
    {
      _generatePassword = generatePassword;
      _repository = repository;
      _cacheOptions = cacheOptions;
      _cache = cache;
      _parser = parser;
      _responseCreator = responseCreator;
      _textTemplateService = textTemplateService;
      _emailService = emailService;
    }

    public async Task<OperationResultResponse<string>> ExecuteAsync(string userLoginData)
    {
      if (string.IsNullOrEmpty(userLoginData))
      {
        return _responseCreator.CreateFailureResponse<string>(HttpStatusCode.BadRequest);
      }

      GetUserFilter filter = CreateFilter(userLoginData);

      DbUser dbUser = await _repository.GetAsync(filter, CommunicationVisibleTo.Admin);

      if (dbUser is null)
      {
        return _responseCreator.CreateFailureResponse<string>(HttpStatusCode.NotFound);
      }

      string secret = SetGuidInCache(dbUser.Id);
      string email = filter.Email is null
        ? dbUser.Communications.FirstOrDefault(c => c.Type == (int)CommunicationType.BaseEmail).Value
        : filter.Email;

      OperationResultResponse<string> response = new();

      await NotifyAsync(dbUser, email, secret, "ru", response.Errors);

      response.Body = response.Errors.Any() ? null : email;

      response.Status = response.Errors.Any()
        ? OperationResultStatusType.Failed
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
