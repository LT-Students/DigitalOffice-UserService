using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Requests.Message;
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
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Validation.User.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace LT.DigitalOffice.UserService.Business.Commands.User
{
  /// <inheritdoc/>
  public class CreateUserCommand : ICreateUserCommand
  {
    private readonly IUserRepository _userRepository;
    private readonly IImageRepository _imageRepository;
    private readonly ILogger<CreateUserCommand> _logger;
    private readonly IRequestClient<ICreateImagesRequest> _rcImage;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRequestClient<IEditCompanyEmployeeRequest> _rcEditCompanyEmployee;
    private readonly IRequestClient<IChangeUserRoleRequest> _rcRole;
    private readonly IRequestClient<ISendEmailRequest> _rcSendEmail;
    private readonly ICreateUserRequestValidator _validator;
    private readonly IDbUserMapper _mapperUser;
    private readonly IDbEntityImageMapper _dbEntityImageMapper;
    private readonly ICreateImageDataMapper _createImageDataMapper;
    private readonly IAccessValidator _accessValidator;
    private readonly IGeneratePasswordCommand _generatePassword;

    #region private methods

    private void EditCompanyEmployee(Guid? departmentId, Guid? positionId, Guid? officeId, Guid userId, List<string> errors)
    {
      string positionErrorMessage = $"Cannot assign position to user. Please try again later.";
      string departmentErrorMessage = $"Cannot assign department to user. Please try again later.";
      string officeErrorMessage = $"Cannot assign office to user. Please try again later.";
      const string departmentLogMessage = "Cannot assign department {departmentId} to user with id {UserId}.";
      const string positionLogMessage = "Cannot assign position {positionId} to user with id {UserId}.";
      const string officeLogMessage = "Cannot assign office {officeId} to user with id {UserId}.";
      const string logMessage = "Cannot edit company employee info for user witd id {UserId}.";

      try
      {
        IOperationResult<(bool department, bool position, bool office)> response =
          _rcEditCompanyEmployee.GetResponse<IOperationResult<(bool department, bool position, bool office)>>(
          IEditCompanyEmployeeRequest.CreateObj(
            userId,
            _httpContextAccessor.HttpContext.GetUserId(),
            departmentId: departmentId,
            positionId: positionId,
            officeId: officeId)).Result.Message;

        if (!response.IsSuccess)
        {
          _logger.LogWarning(logMessage, userId);

          if (departmentId.HasValue)
          {
            errors.Add(departmentErrorMessage);
          }

          if (positionId.HasValue)
          {
            errors.Add(positionErrorMessage);
          }

          if (officeId.HasValue)
          {
            errors.Add(officeErrorMessage);
          }

          return;
        }

        if (!response.Body.department)
        {
          _logger.LogWarning(departmentLogMessage, userId, departmentId);

          errors.Add(departmentErrorMessage);
        }

        if (!response.Body.position)
        {
          _logger.LogWarning(positionLogMessage, userId, positionId);

          errors.Add(positionErrorMessage);
        }

        if (!response.Body.office)
        {
          _logger.LogWarning(officeLogMessage, userId, officeId);

          errors.Add(officeErrorMessage);
        }
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage, userId);

        if (departmentId.HasValue)
        {
          errors.Add(departmentErrorMessage);
        }

        if (positionId.HasValue)
        {
          errors.Add(positionErrorMessage);
        }

        if (officeId.HasValue)
        {
          errors.Add(officeErrorMessage);
        }
      }
    }

    private void ChangeUserRole(Guid roleId, Guid userId, List<string> errors)
    {
      string errorMessage = $"Can't assign role '{roleId}' to the user '{userId}'. Please try again later.";
      const string logMessage = "Can't assign role '{RoleId}' to the user '{UserId}'";

      try
      {
        var response = _rcRole.GetResponse<IOperationResult<bool>>(
          IChangeUserRoleRequest.CreateObj(
            roleId,
            userId,
            _httpContextAccessor.HttpContext.GetUserId())).Result;
        if (!response.Message.IsSuccess || !response.Message.Body)
        {
          _logger.LogWarning(logMessage, roleId, userId);

          errors.Add(errorMessage);
        }
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage, roleId, userId);

        errors.Add(errorMessage);
      }
    }

    private void SendEmail(DbUser dbUser, string password, List<string> errors)
    {
      var email = dbUser.Communications.FirstOrDefault(c => c.Type == (int)CommunicationType.Email);

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
        var templateValues = ISendEmailRequest.CreateTemplateValuesDictionary(
          userFirstName: dbUser.FirstName,
          userEmail: email.Value,
          userId: dbUser.Id.ToString(),
          userPassword: password);

        var emailRequest = ISendEmailRequest.CreateObj(null, senderId, email.Value, templateLanguage, templateType, templateValues);

        IOperationResult<bool> rcSendEmailResponse = _rcSendEmail
          .GetResponse<IOperationResult<bool>>(emailRequest, timeout: RequestTimeout.Default)
          .Result
          .Message;

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

    private Guid? GetAvatarImageId(AddImageRequest avatarRequest, List<string> errors)
    {
      if (avatarRequest == null)
      {
        return null;
      }

      Guid? avatarImageId = null;
      Guid userId = _httpContextAccessor.HttpContext.GetUserId();

      string errorMessage = $"Can not add avatar image to user with id {userId}. Please try again later.";

      try
      {
        Response<IOperationResult<ICreateImagesResponse>> createResponse = _rcImage.GetResponse<IOperationResult<ICreateImagesResponse>>(
          ICreateImagesRequest.CreateObj(
            _createImageDataMapper.Map(new List<AddImageRequest>() { avatarRequest }, userId),
            ImageSource.User))
          .Result;

        if (!createResponse.Message.IsSuccess)
        {
          _logger.LogWarning(
            "Can not add avatar image to user with id {UserId}. Reason: '{Errors}'",
            userId,
            string.Join(',', createResponse.Message.Errors));

          errors.Add(errorMessage);
        }
        else
        {
          avatarImageId = createResponse.Message.Body.ImagesIds.FirstOrDefault();
        }
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, "Can not add avatar image to user with id {UserId}", userId);

        errors.Add(errorMessage);
      }

      return avatarImageId;
    }
    #endregion

    public CreateUserCommand(
      ILogger<CreateUserCommand> logger,
      IRequestClient<ICreateImagesRequest> rcImage,
      IHttpContextAccessor httpContextAccessor,
      IRequestClient<IEditCompanyEmployeeRequest> rcEditCompanyEmployee,
      IRequestClient<IChangeUserRoleRequest> rcRole,
      IRequestClient<ISendEmailRequest> rcSendEmail,
      IUserRepository userRepository,
      ICreateUserRequestValidator validator,
      IDbUserMapper mapperUser,
      IDbEntityImageMapper dbEntityImageMapper,
      ICreateImageDataMapper createImageDataMapper,
      IAccessValidator accessValidator,
      IGeneratePasswordCommand generatePassword,
      IImageRepository imageRepository)
    {
      _logger = logger;
      _rcImage = rcImage;
      _rcEditCompanyEmployee = rcEditCompanyEmployee;
      _rcRole = rcRole;
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
    }

    /// <inheritdoc/>
    public OperationResultResponse<Guid> Execute(CreateUserRequest request)
    {
      OperationResultResponse<Guid> response = new();

      if (!(_accessValidator.IsAdmin() ||
        _accessValidator.HasRights(Rights.AddEditRemoveUsers)))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        response.Status = OperationResultStatusType.Failed;
        response.Errors.Add("Not enough rights.");

        return response;
      }

      _validator.ValidateAndThrowCustom(request);

      if (_userRepository.IsCommunicationValueExist(request.Communications.Select(x => x.Value).ToList()))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
        response.Status = OperationResultStatusType.Failed;
        response.Errors.Add("Communication value already exist");

        return response;
      }

      Guid? avatarImageId = GetAvatarImageId(request.AvatarImage, response.Errors);

      DbUser dbUser = _mapperUser.Map(request, avatarImageId);

      string password = !string.IsNullOrEmpty(request.Password?.Trim()) ? request.Password.Trim() : _generatePassword.Execute();

      Guid userId = _userRepository.Create(dbUser);
      _userRepository.CreatePending(new DbPendingUser() { UserId = dbUser.Id, Password = password });

      if (avatarImageId.HasValue)
      {
        _imageRepository.Create(_dbEntityImageMapper.Map(new List<Guid>() { avatarImageId.Value }, userId));
      }

      SendEmail(dbUser, password, response.Errors);

      EditCompanyEmployee(request.DepartmentId, request.PositionId, request.OfficeId, dbUser.Id, response.Errors);

      if (request.RoleId.HasValue)
      {
        ChangeUserRole(request.RoleId.Value, userId, response.Errors);
      }

      response.Body = userId;
      response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;
      return response;
    }
  }
}