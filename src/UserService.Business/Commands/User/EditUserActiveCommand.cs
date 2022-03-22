﻿using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.UserService.Business.Commands.Password.Interfaces;
using LT.DigitalOffice.UserService.Business.Commands.User.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Validation.User.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using FluentValidation.Results;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using LT.DigitalOffice.Models.Broker.Responses.TextTemplate;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Kernel.Helpers.TextHandlers.Interfaces;
using LT.DigitalOffice.Kernel.Enums;

namespace LT.DigitalOffice.UserService.Business.Commands.User
{
  /// <inheritdoc/>
  public class EditUserActiveCommand : IEditUserActiveCommand
  {
    private readonly IEditUserActiveRequestValidator _validator;
    private readonly IUserRepository _userRepository;
    private readonly IPendingUserRepository _pendingRepository;
    private readonly IUserCommunicationRepository _communicationRepository;
    private readonly IGeneratePasswordCommand _generatePassword;
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IResponseCreator _responseCreator;
    private readonly IBus _bus;
    private readonly ITextTemplateService _textTemplateService;
    private readonly IEmailService _emailService;
    private readonly ITextTemplateParser _parser;

    private async Task NotifyAsync(
      DbUser dbUser,
      string email,
      string password,
      string locale,
      List<string> errors)
    {
      IGetTextTemplateResponse textTemplate = await _textTemplateService
        .GetAsync(TemplateType.UserRecovery, locale, errors);

      if (textTemplate is null)
      {
        return;
      }

      string parsedText = _parser.Parse(
        new Dictionary<string, string> { { "Password", password } },
        _parser.ParseModel<DbUser>(dbUser, textTemplate.Text));

      await _emailService.SendAsync(email, textTemplate.Subject, parsedText, errors);
    }

    public EditUserActiveCommand(
      IEditUserActiveRequestValidator validator,
      IUserRepository userRepository,
      IPendingUserRepository pendingRepository,
      IUserCommunicationRepository communicationRepository,
      IGeneratePasswordCommand generatePassword,
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IResponseCreator responseCreator,
      IBus bus,
      ITextTemplateService textTemplateService,
      IEmailService emailService,
      ITextTemplateParser parser)
    {
      _validator = validator;
      _userRepository = userRepository;
      _pendingRepository = pendingRepository;
      _communicationRepository = communicationRepository;
      _generatePassword = generatePassword;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _responseCreator = responseCreator;
      _bus = bus;
      _textTemplateService = textTemplateService;
      _emailService = emailService;
      _parser = parser;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(EditUserActiveRequest request)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers)
        || _httpContextAccessor.HttpContext.GetUserId() == request.UserId)
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator
        .ValidateAsync(request);

      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest,
          validationResult.Errors.Select(vf => vf.ErrorMessage).ToList());
      }

      OperationResultResponse<bool> response = new();

      if (!request.IsActive)
      {
        response.Body = await _userRepository.SwitchActiveStatusAsync(request.UserId, request.IsActive);

        await _bus.Publish<IDisactivateUserRequest>(IDisactivateUserRequest.CreateObj(
          request.UserId,
          _httpContextAccessor.HttpContext.GetUserId()));
      }
      else
      {
        string password = _generatePassword.Execute();

        await _pendingRepository.CreateAsync(
          new DbPendingUser()
          {
            UserId = request.UserId,
            Password = password,
            CommunicationId = request.CommunicationId.HasValue 
              ? request.CommunicationId.Value
              : (await _communicationRepository.GetBaseAsync(request.UserId)).Id
          });

        response.Body = true;

        DbUser dbUser = await _userRepository.GetAsync(request.UserId, true);

        await NotifyAsync(
          dbUser,
          dbUser.Communications.FirstOrDefault(c => c.Id == request.CommunicationId)?.Value,
          password,
          "ru",
          response.Errors);
      }

      response.Status = response.Errors.Any() 
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}