using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
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
      if (!departmentId.HasValue || departmentId.Value == Guid.Empty)
      {
        return;
      }

      try
      {
        Response<IOperationResult<bool>> response = await _rcCreateDepartmentEntity.GetResponse<IOperationResult<bool>>(
          ICreateDepartmentEntityRequest.CreateObj(
            departmentId: departmentId.Value,
            createdBy: _httpContextAccessor.HttpContext.GetUserId(),
            userId: userId));

        if (response.Message.IsSuccess && response.Message.Body)
        {
          return;
        }

        _logger.LogWarning(
          "Error while adding user id {UserId} to the department id {DepartmentId}.\nErrors: {Errors}",
          userId,
          departmentId,
          string.Join('\n', response.Message.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(
          exc,
          "Cannot add user id {UserId} to the department id {DepartmentId}",
          userId,
          departmentId);
      }

      errors.Add("Unable to enroll a user in the department. Please try again later.");
    }

    private async Task CreateUserPositionAsync(Guid? positionId, Guid userId, List<string> errors)
    {
      if (!positionId.HasValue || positionId.Value == Guid.Empty)
      {
        return;
      }

      try
      {
        Response<IOperationResult<bool>> response = await _rcCreateUserPosition.GetResponse<IOperationResult<bool>>(
          ICreateUserPositionRequest.CreateObj(
            positionId: positionId.Value,
            createdBy: _httpContextAccessor.HttpContext.GetUserId(),
            userId: userId));

        if (response.Message.IsSuccess && response.Message.Body)
        {
          return;
        }

        _logger.LogWarning(
          "Error while adding user id {UserId} to the position id {PositionId}.\nErrors: {Errors}",
          userId,
          positionId,
          string.Join('\n', response.Message.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(
          exc,
          "Cannot add user id {UserId} to the position id {PositionId}.",
          userId,
          positionId);
      }

      errors.Add("Cannot assign position to user. Please try again later.");
    }

    private async Task CreateUserCompanyAsync(Guid? companyId, Guid userId, double? rate, DateTime? startWorkingAt, List<string> errors)
    {
      if (!companyId.HasValue || companyId.Value == Guid.Empty)
      {
        return;
      }

      try
      {
        Response<IOperationResult<bool>> response = await _rcCreateUserPosition.GetResponse<IOperationResult<bool>>(
          ICreateCompanyUserRequest.CreateObj(
            companyId: companyId.Value,
            userId: userId,
            rate: rate,
            startWorkingAt: startWorkingAt,
            createdBy: _httpContextAccessor.HttpContext.GetUserId()));

        if (response.Message.IsSuccess && response.Message.Body)
        {
          return;
        }

        _logger.LogWarning(
          "Error while adding user id {UserId} to the company id {CompanyId}.\nErrors: {Errors}",
          userId,
          companyId,
          string.Join('\n', response.Message.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(
          exc,
          "Cannot add user id {UserId} to the company id {CompanyId}.",
          userId,
          companyId);
      }

      errors.Add("Cannot assign company to user. Please try again later.");
    }

    private async Task CreateUserOfficeAsync(Guid? officeId, Guid userId, List<string> errors)
    {
      if (!officeId.HasValue || officeId.Value == Guid.Empty)
      {
        return;
      }

      try
      {
        Response<IOperationResult<bool>> response =
          await _rcCreateUserOffice.GetResponse<IOperationResult<bool>>(
            ICreateUserOfficeRequest.CreateObj(
              userId: userId,
              modifiedBy: _httpContextAccessor.HttpContext.GetUserId(),
              officeId: officeId.Value));

        if (response.Message.IsSuccess && response.Message.Body)
        {
          return;
        }

        _logger.LogWarning(
          "Error while adding user id {UserId} to the office id {OfficeId}.\nErrors: {Errors}",
          userId,
          officeId,
          string.Join('\n', response.Message.Errors));
      }
      catch (Exception exc)
      {
        _logger.LogError(
          exc,
          "Cannot add user id {UserId} to the office id {OfficeId}.",
          userId,
          officeId);
      }

      errors.Add("Cannot assign office to user. Please try again later.");
    }

    private async Task ChangeUserRoleAsync(Guid? roleId, Guid userId, List<string> errors)
    {
      if (!roleId.HasValue || roleId.Value == Guid.Empty)
      {
        return;
      }

      try
      {
        Response<IOperationResult<bool>> response =
          await _rcCreateUserRole.GetResponse<IOperationResult<bool>>(
            IChangeUserRoleRequest.CreateObj(
              roleId.Value, userId, _httpContextAccessor.HttpContext.GetUserId()));

        if (!response.Message.IsSuccess || !response.Message.Body)
        {
          _logger.LogWarning(
            "Error while adding user id {UserId} to the role id {RoleId}.\nErrors: {Errors}",
            userId,
            roleId,
            string.Join('\n', response.Message.Errors));
        }
      }
      catch (Exception exc)
      {
        _logger.LogError(
          exc,
          "Cannot add user id {UserId} to the role id {RoleId}.",
          userId,
          roleId);
      }

      errors.Add("Cannot assign role to user. Please try again later.");
    }

    private async Task SendEmailAsync(DbUser dbUser, string password, List<string> errors)
    {
      DbUserCommunication email = dbUser.Communications.FirstOrDefault(c => c.Type == (int)CommunicationType.Email);

      if (email is null)
      {
        errors.Add("User does not have any linked email.");

        return;
      }

      try
      {
        Response<IOperationResult<IGetTextTemplateResponse>> textTemplateResponse =
          await _rcGetTextTemplate.GetResponse<IOperationResult<IGetTextTemplateResponse>>(
            IGetTextTemplateRequest.CreateObj(
              endpointId: Guid.Empty,
              templateType: TemplateType.Greeting,
              locale: "en"));

        if (!textTemplateResponse.Message.IsSuccess || textTemplateResponse.Message.Body is null)
        {
          _logger.LogWarning(
            "Errors while getting text template':\n {Errors}",
            string.Join(Environment.NewLine, textTemplateResponse.Message.Errors));

          errors.Add("Email template not found");
          return;
        }

        string parsedText = _parser.Parse(
          new Dictionary<string, string> { { "Password", password } },
          _parser.ParseModel<DbUser>(dbUser, textTemplateResponse.Message.Body.Text));

        Response<IOperationResult<bool>> sendEmailResponse =
          await _rcSendEmail.GetResponse<IOperationResult<bool>>(
            ISendEmailRequest.CreateObj(
              _httpContextAccessor.HttpContext.GetUserId(),
              email.Value,
              textTemplateResponse.Message.Body.Subject,
              parsedText));

        if (!sendEmailResponse.Message.IsSuccess || !sendEmailResponse.Message.Body)
        {
          _logger.LogWarning(
            "Errors while sending email to '{Email}':\n {Errors}",
            email.Value,
            string.Join(Environment.NewLine, sendEmailResponse.Message.Errors));

          errors.Add($"Can not send email to '{email.Value}'. Email placed in resend queue and will be resent in 1 hour.");
        }
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, "Can not send email to '{Email}'", email.Value);

        errors.Add($"Can not send email to '{email.Value}'. Email placed in resend queue and will be resent in 1 hour.");
      }
    }

    private async Task<Guid?> GetAvatarImageIdAsync(CreateAvatarRequest avatarRequest, List<string> errors)
    {
      if (avatarRequest == null)
      {
        return null;
      }

      Guid? avatarImageId = null;
      Guid senderId = _httpContextAccessor.HttpContext.GetUserId();

      string errorMessage = $"Can not add avatar image to user. Please try again later.";

      try
      {
        Response<IOperationResult<ICreateImagesResponse>> createResponse =
          await _rcCreateImage.GetResponse<IOperationResult<ICreateImagesResponse>>(
            ICreateImagesRequest.CreateObj(
              _createImageDataMapper.Map(new List<CreateAvatarRequest>() { avatarRequest }),
              ImageSource.User));

        if (createResponse.Message.IsSuccess
          && createResponse.Message.Body != null
          && createResponse.Message.Body.ImagesIds != null)
        {
          return createResponse.Message.Body.ImagesIds.FirstOrDefault();
        }

        _logger.LogWarning(
          "Can not add avatar image to user, senderId: {SenderId}. Reason: '{Errors}'",
          senderId,
          string.Join(',', createResponse.Message.Errors));

        errors.Add(errorMessage);
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, "Can not add avatar image to user, senderId:{SenderId}", senderId);

        errors.Add(errorMessage);
      }

      return avatarImageId;
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

      Guid userId = dbUser.Id;//await _userRepository.CreateAsync(dbUser);

      //await _pendingUserRepository.CreateAsync(new DbPendingUser() { UserId = userId, Password = password });

      Guid? avatarImageId = await GetAvatarImageIdAsync(request.AvatarImage, response.Errors);
      if (avatarImageId.HasValue)
      {
        await _imageRepository.CreateAsync(_dbUserAvatarMapper.Map(avatarImageId.Value, userId, true));
      }

      await Task.WhenAll(
        SendEmailAsync(dbUser, password, response.Errors),
        CreateUserOfficeAsync(request.OfficeId, userId, response.Errors),
        ChangeUserRoleAsync(request.RoleId, userId, response.Errors),
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