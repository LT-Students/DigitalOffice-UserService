﻿using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.EndpointSupport.Constants.Endpoints;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Helpers.TextHandlers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Responses.TextTemplate;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using LT.DigitalOffice.UserService.Business.Commands.Avatar.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Avatar;
using LT.DigitalOffice.UserService.Validation.Image.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Avatar
{
  public class CreateAvatarCommand : ICreateAvatarCommand
  {
    private readonly IUserAvatarRepository _avatarRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAccessValidator _accessValidator;
    private readonly ICreateAvatarRequestValidator _requestValidator;
    private readonly IDbUserAvatarMapper _dbUserAvatarMapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IImageService _imageService;
    private readonly IResponseCreator _responseCreator;
    private readonly ITextTemplateService _textTemplateService;
    private readonly IEmailService _emailService;
    private readonly ITextTemplateParser _parser;

    public CreateAvatarCommand(
      IUserAvatarRepository avatarRepository,
      IUserRepository userRepository,
      IAccessValidator accessValidator,
      ICreateAvatarRequestValidator requestValidator,
      IDbUserAvatarMapper dbEntityImageMapper,
      IHttpContextAccessor httpContextAccessor,
      IImageService imageService,
      IResponseCreator responseCreator,
      ITextTemplateService textTemplateService,
      IEmailService emailService,
      ITextTemplateParser parser)
    {
      _avatarRepository = avatarRepository;
      _userRepository = userRepository;
      _accessValidator = accessValidator;
      _requestValidator = requestValidator;
      _dbUserAvatarMapper = dbEntityImageMapper;
      _httpContextAccessor = httpContextAccessor;
      _imageService = imageService;
      _responseCreator = responseCreator;
      _textTemplateService = textTemplateService;
      _emailService = emailService;
      _parser = parser;
    }

    public async Task<OperationResultResponse<Guid?>> ExecuteAsync(CreateAvatarRequest request)
    {
      if (_httpContextAccessor.HttpContext.GetUserId() != request.UserId
        && !await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers))
      {
        return _responseCreator.CreateFailureResponse<Guid?>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _requestValidator.ValidateAsync(request);

      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<Guid?>(
          HttpStatusCode.BadRequest,
          validationResult.Errors.Select(validationFailure => validationFailure.ErrorMessage).ToList());
      }

      OperationResultResponse<Guid?> response = new();

      response.Body = await _imageService.CreateImageAsync(request, response.Errors);

      if (response.Body is not null)
      {
        await _avatarRepository.CreateAsync(
          _dbUserAvatarMapper
            .Map(response.Body.Value, request.UserId.Value, request.IsCurrentAvatar));

        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

        response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;
      }
      else
      {
        response = _responseCreator.CreateFailureResponse<Guid?>(HttpStatusCode.BadRequest);
      }

      return response;
    }
  }
}
