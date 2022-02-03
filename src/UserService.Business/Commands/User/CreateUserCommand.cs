using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Helpers.TextHandlers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.Department;
using LT.DigitalOffice.Models.Broker.Requests.Email;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Requests.Office;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.Models.Broker.Requests.TextTemplate;
using LT.DigitalOffice.Models.Broker.Responses.Image;
using LT.DigitalOffice.Models.Broker.Responses.TextTemplate;
using LT.DigitalOffice.UserService.Business.Commands.Password.Interfaces;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Avatar;
using LT.DigitalOffice.UserService.Validation.User.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.User
{
  /// <inheritdoc/>
  public class CreateUserCommand : ICreateUserCommand
  {
    private readonly ICreateUserRequestValidator _validator;
    private readonly IUserRepository _userRepository;
    private readonly IPendingUserRepository _pendingUserRepository;
    private readonly IAvatarRepository _imageRepository;
    private readonly IDbUserMapper _dbUserMapper;
    private readonly IDbUserAvatarMapper _dbUserAvatarMapper;
    private readonly ICreateImageDataMapper _createImageDataMapper;
    private readonly IAccessValidator _accessValidator;
    private readonly IGeneratePasswordCommand _generatePassword;
    private readonly IResponseCreator _responseCreator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITextTemplateParser _parser;
    private readonly ILogger<CreateUserCommand> _logger;
    private readonly IRequestClient<ICreateCompanyUserRequest> _rcCreateCompanyUser;
    private readonly IRequestClient<ICreateDepartmentEntityRequest> _rcCreateDepartmentEntity;
    private readonly IRequestClient<ICreateImagesRequest> _rcCreateImage;
    private readonly IRequestClient<ICreateUserOfficeRequest> _rcCreateUserOffice;
    private readonly IRequestClient<ICreateUserPositionRequest> _rcCreateUserPosition;
    private readonly IRequestClient<IChangeUserRoleRequest> _rcCreateUserRole;
    private readonly IRequestClient<IGetTextTemplateRequest> _rcGetTextTemplate;
    private readonly IRequestClient<ISendEmailRequest> _rcSendEmail;

    #region private methods

    private async Task CreateDepartmentUserAsync(Guid? departmentId, Guid userId, List<string> errors)
    {
      if (departmentId.HasValue && departmentId.Value != Guid.Empty)
      {
        await RequestHandler.ProcessRequest<ICreateDepartmentEntityRequest, bool>(
          _rcCreateDepartmentEntity,
          ICreateDepartmentEntityRequest.CreateObj(
            departmentId: departmentId.Value,
            createdBy: _httpContextAccessor.HttpContext.GetUserId(),
            userId: userId),
          errors,
          _logger);
      }
    }

    private async Task CreateUserPositionAsync(Guid? positionId, Guid userId, List<string> errors)
    {
      if (positionId.HasValue && positionId.Value != Guid.Empty)
      {
        await RequestHandler.ProcessRequest<ICreateUserPositionRequest, bool>(
          _rcCreateUserPosition,
          ICreateUserPositionRequest.CreateObj(
            positionId: positionId.Value,
            createdBy: _httpContextAccessor.HttpContext.GetUserId(),
            userId: userId),
          errors,
          _logger);
      }
    }

    private async Task CreateUserCompanyAsync(Guid? companyId, Guid userId, double? rate, DateTime? startWorkingAt, List<string> errors)
    {
      if (companyId.HasValue && companyId.Value != Guid.Empty)
      {
        await RequestHandler.ProcessRequest<ICreateCompanyUserRequest, bool>(
          _rcCreateCompanyUser,
          ICreateCompanyUserRequest.CreateObj(
            companyId: companyId.Value,
            userId: userId,
            rate: rate,
            startWorkingAt: startWorkingAt,
            createdBy: _httpContextAccessor.HttpContext.GetUserId()),
          errors,
          _logger);
      }
    }

    private async Task CreateUserOfficeAsync(Guid? officeId, Guid userId, List<string> errors)
    {
      if (officeId.HasValue && officeId.Value != Guid.Empty)
      {
        await RequestHandler.ProcessRequest<ICreateUserOfficeRequest, bool>(
          _rcCreateUserOffice,
          ICreateUserOfficeRequest.CreateObj(
            userId: userId,
            modifiedBy: _httpContextAccessor.HttpContext.GetUserId(),
            officeId: officeId.Value),
          errors,
          _logger);
      }
    }

    private async Task CreateUserRoleAsync(Guid? roleId, Guid userId, List<string> errors)
    {
      if (roleId.HasValue && roleId.Value != Guid.Empty)
      {
        await RequestHandler.ProcessRequest<IChangeUserRoleRequest, bool>(
          _rcCreateUserRole,
          IChangeUserRoleRequest.CreateObj(
            roleId.Value,
            userId, _httpContextAccessor.HttpContext.GetUserId()),
          errors,
          _logger);
      }
    }

    private async Task SendEmailAsync(DbUser dbUser, string password, List<string> errors)
    {
      DbUserCommunication email = dbUser.Communications.FirstOrDefault(c => c.Type == (int)CommunicationType.Email);

      if (email is null)
      {
        errors.Add("User does not have any linked email.");

        return;
      }

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
          dbUser.Id);

        return;
      }

      string parsedText = _parser.Parse(
        new Dictionary<string, string> { { "Password", password } },
        _parser.ParseModel<DbUser>(dbUser, textTemplate.Text));

      if (!await RequestHandler.ProcessRequest<ISendEmailRequest, bool>(
        _rcSendEmail,
        ISendEmailRequest.CreateObj(
          email.Value,
          textTemplate.Subject,
          parsedText,
          _httpContextAccessor.HttpContext.GetUserId()),
        errors,
        _logger))
      {
        _logger.LogError(
          "Invitation letter not sent to email '{Email}'",
          email);

        errors.Add($"Can not send email to '{email.Value}'. Email placed in resend queue and will be resent in 1 hour.");
      }
    }

    private async Task<Guid?> GetAvatarImageIdAsync(CreateAvatarRequest avatarRequest, List<string> errors)
    {
      if (avatarRequest is null)
      {
        return null;
      }

      return (await RequestHandler.ProcessRequest<ICreateImagesRequest, ICreateImagesResponse>(
          _rcCreateImage,
          ICreateImagesRequest.CreateObj(
            _createImageDataMapper.Map(new List<CreateAvatarRequest>() { avatarRequest }),
            ImageSource.User),
          errors,
          _logger))
        ?.ImagesIds.FirstOrDefault();
    }
    #endregion

    public CreateUserCommand(
      ICreateUserRequestValidator validator,
      IUserRepository userRepository,
      IPendingUserRepository pendingUserRepository,
      IAvatarRepository imageRepository,
      IHttpContextAccessor httpContextAccessor,
      IDbUserMapper dbUserMapper,
      IDbUserAvatarMapper dbUserAvatarMapper,
      ICreateImageDataMapper createImageDataMapper,
      IAccessValidator accessValidator,
      IGeneratePasswordCommand generatePassword,
      IResponseCreator responseCreator,
      ITextTemplateParser parser,
      ILogger<CreateUserCommand> logger,
      IRequestClient<ICreateCompanyUserRequest> rcCreateCompanyUser,
      IRequestClient<ICreateDepartmentEntityRequest> rcCreateDepartmentEntity,
      IRequestClient<ICreateImagesRequest> rcCreateImage,
      IRequestClient<IChangeUserRoleRequest> rcCreateUserRole,
      IRequestClient<ICreateUserOfficeRequest> rcCreateUserOffice,
      IRequestClient<ICreateUserPositionRequest> rcCreateUserPosition,
      IRequestClient<IGetTextTemplateRequest> rcGetTextTemplate,
      IRequestClient<ISendEmailRequest> rcSendEmail)
    {
      _validator = validator;
      _userRepository = userRepository;
      _pendingUserRepository = pendingUserRepository;
      _imageRepository = imageRepository;
      _dbUserMapper = dbUserMapper;
      _dbUserAvatarMapper = dbUserAvatarMapper;
      _createImageDataMapper = createImageDataMapper;
      _accessValidator = accessValidator;
      _generatePassword = generatePassword;
      _httpContextAccessor = httpContextAccessor;
      _responseCreator = responseCreator;
      _parser = parser;
      _logger = logger;
      _rcCreateCompanyUser = rcCreateCompanyUser;
      _rcCreateDepartmentEntity = rcCreateDepartmentEntity;
      _rcCreateImage = rcCreateImage;
      _rcCreateUserRole = rcCreateUserRole;
      _rcCreateUserOffice = rcCreateUserOffice;
      _rcCreateUserPosition = rcCreateUserPosition;
      _rcGetTextTemplate = rcGetTextTemplate;
      _rcSendEmail = rcSendEmail;
    }

    public async Task<OperationResultResponse<Guid>> ExecuteAsync(CreateUserRequest request)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers))
      {
        return _responseCreator.CreateFailureResponse<Guid>(HttpStatusCode.Forbidden);
      }

      if (!_validator.ValidateCustom(request, out List<string> errors))
      {
        return _responseCreator.CreateFailureResponse<Guid>(HttpStatusCode.BadRequest, errors);
      }

      OperationResultResponse<Guid> response = new();

      DbUser dbUser = _dbUserMapper.Map(request);

      string password = !string.IsNullOrEmpty(request.Password?.Trim()) ?
        request.Password.Trim() : _generatePassword.Execute();

      Guid userId = await _userRepository.CreateAsync(dbUser);

      await _pendingUserRepository.CreateAsync(new DbPendingUser() { UserId = userId, Password = password });

      Guid? avatarImageId = await GetAvatarImageIdAsync(request.AvatarImage, response.Errors);
      if (avatarImageId.HasValue)
      {
        await _imageRepository.CreateAsync(_dbUserAvatarMapper.Map(avatarImageId.Value, userId, true));
      }

      await Task.WhenAll(
        SendEmailAsync(dbUser, password, response.Errors),
        CreateUserOfficeAsync(request.OfficeId, userId, response.Errors),
        CreateUserRoleAsync(request.RoleId, userId, response.Errors),
        CreateDepartmentUserAsync(request.DepartmentId, userId, response.Errors),
        CreateUserPositionAsync(request.PositionId, userId, response.Errors),
        CreateUserCompanyAsync(request.CompanyId, userId, request.Rate, request.StartWorkingAt, response.Errors));

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;
      response.Body = userId;
      response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}