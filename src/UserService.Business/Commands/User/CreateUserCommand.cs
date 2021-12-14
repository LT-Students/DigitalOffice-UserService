using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.Department;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Requests.Message;
using LT.DigitalOffice.Models.Broker.Requests.Office;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.Models.Broker.Responses.Image;
using LT.DigitalOffice.UserService.Business.Commands.Password.Interfaces;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Avatar;
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
    private readonly IUserRepository _userRepository;
    private readonly IAvatarRepository _imageRepository;
    private readonly ILogger<CreateUserCommand> _logger;
    private readonly IRequestClient<ICreateImagesRequest> _rcImage;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRequestClient<ICreateUserOfficeRequest> _rcCreateUserOffice;
    private readonly IRequestClient<ICreateUserPositionRequest> _rcCreateUserPosition;
    private readonly IRequestClient<ICreateDepartmentEntityRequest> _rcCreateDepartmentEntity;
    private readonly IRequestClient<IChangeUserRoleRequest> _rcRole;
    private readonly IRequestClient<ISendEmailRequest> _rcSendEmail;
    private readonly ICreateUserRequestValidator _validator;
    private readonly IDbUserMapper _mapperUser;
    private readonly IDbUserAvatarMapper _dbEntityImageMapper;
    private readonly ICreateImageDataMapper _createImageDataMapper;
    private readonly IAccessValidator _accessValidator;
    private readonly IGeneratePasswordCommand _generatePassword;
    private readonly IResponseCreator _responseCreator;

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
          await _rcRole.GetResponse<IOperationResult<bool>>(
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

      if (email == null)
      {
        errors.Add("User does not have any linked email.");

        return;
      }

      string errorMessage = $"Can not send email to '{email.Value}'. Email placed in resend queue and will be resent in 1 hour.";

      //TODO: fix add specific template language
      string templateLanguage = "en";
      var senderId = _httpContextAccessor.HttpContext.GetUserId();
      EmailTemplateType templateType = EmailTemplateType.Greeting;
      try
      {
        Dictionary<string, string> templateValues =
          ISendEmailRequest.CreateTemplateValuesDictionary(
            userFirstName: dbUser.FirstName,
            userEmail: email.Value,
            userId: dbUser.Id.ToString(),
            userPassword: password);

        object emailRequest = ISendEmailRequest.CreateObj(null, senderId, email.Value, templateLanguage, templateType, templateValues);

        IOperationResult<bool> rcSendEmailResponse = (await _rcSendEmail
          .GetResponse<IOperationResult<bool>>(emailRequest)).Message;

        if (!(rcSendEmailResponse.IsSuccess && rcSendEmailResponse.Body))
        {
          _logger.LogWarning(
            "Errors while sending email to '{Email}':\n {Errors}",
            email.Value,
            string.Join(Environment.NewLine, rcSendEmailResponse.Errors));

          errors.Add(errorMessage);
        }
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, "Can not send email to '{Email}'", email.Value);

        errors.Add(errorMessage);
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
          await _rcImage.GetResponse<IOperationResult<ICreateImagesResponse>>(
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
      ILogger<CreateUserCommand> logger,
      IRequestClient<IChangeUserRoleRequest> rcRole,
      IRequestClient<ICreateImagesRequest> rcImage,
      IRequestClient<ICreateUserOfficeRequest> rcCreateUserOffice,
      IRequestClient<ICreateUserPositionRequest> rcCreateUserPosition,
      IRequestClient<ICreateDepartmentEntityRequest> rcCreateDepartmentEntity,
      IHttpContextAccessor httpContextAccessor,
      IRequestClient<ISendEmailRequest> rcSendEmail,
      IUserRepository userRepository,
      ICreateUserRequestValidator validator,
      IDbUserMapper mapperUser,
      IDbUserAvatarMapper dbEntityImageMapper,
      ICreateImageDataMapper createImageDataMapper,
      IAccessValidator accessValidator,
      IGeneratePasswordCommand generatePassword,
      IAvatarRepository imageRepository,
      IResponseCreator responseCreator)
    {
      _logger = logger;
      _rcRole = rcRole;
      _rcImage = rcImage;
      _rcCreateUserOffice = rcCreateUserOffice;
      _rcCreateUserPosition = rcCreateUserPosition;
      _rcCreateDepartmentEntity = rcCreateDepartmentEntity;
      _rcSendEmail = rcSendEmail;
      _validator = validator;
      _httpContextAccessor = httpContextAccessor;
      _userRepository = userRepository;
      _mapperUser = mapperUser;
      _dbEntityImageMapper = dbEntityImageMapper;
      _createImageDataMapper = createImageDataMapper;
      _accessValidator = accessValidator;
      _generatePassword = generatePassword;
      _imageRepository = imageRepository;
      _responseCreator = responseCreator;
    }

    /// <inheritdoc/>
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

      DbUser dbUser = _mapperUser.Map(request);

      string password = !string.IsNullOrEmpty(request.Password?.Trim()) ?
        request.Password.Trim() : _generatePassword.Execute();

      Guid userId = await _userRepository.CreateAsync(dbUser);

      await _userRepository.CreatePendingAsync(new DbPendingUser() { UserId = userId, Password = password });

      Guid? avatarImageId = await GetAvatarImageIdAsync(request.AvatarImage, response.Errors);
      if (avatarImageId.HasValue)
      {
        await _imageRepository.CreateAsync(_dbEntityImageMapper.Map(avatarImageId.Value, userId, true));
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