using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Helpers.TextHandlers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Company;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Department;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Office;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Position;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Right;
using LT.DigitalOffice.Models.Broker.Responses.TextTemplate;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using LT.DigitalOffice.UserService.Business.Commands.Password.Interfaces;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Validation.User.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
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
    private readonly IUserAvatarRepository _imageRepository;
    private readonly IDbUserMapper _dbUserMapper;
    private readonly IDbUserAvatarMapper _dbUserAvatarMapper;
    private readonly IAccessValidator _accessValidator;
    private readonly IGeneratePasswordCommand _generatePassword;
    private readonly IResponseCreator _responseCreator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITextTemplateParser _parser;
    private readonly IImageService _imageService;
    private readonly ITextTemplateService _textTemplateService;
    private readonly IEmailService _emailService;
    private readonly IBus _bus;

    private async Task NotifyAsync(
      DbUser dbUser,
      string password,
      string locale,
      List<string> errors)
    {
      IGetTextTemplateResponse textTemplate = await _textTemplateService
        .GetAsync(TemplateType.Greeting, locale, errors);

      if (textTemplate is null)
      {
        return;
      }

      string parsedText = _parser.Parse(
        new Dictionary<string, string> { { "Password", password } },
        _parser.ParseModel<DbUser>(dbUser, textTemplate.Text));

      string email = dbUser.Communications
        .FirstOrDefault(c => c.Type == (int)CommunicationType.Email)?.Value;

      await _emailService.SendAsync(email, textTemplate.Subject, parsedText, errors);
    }

    public CreateUserCommand(
      ICreateUserRequestValidator validator,
      IUserRepository userRepository,
      IPendingUserRepository pendingUserRepository,
      IUserAvatarRepository imageRepository,
      IHttpContextAccessor httpContextAccessor,
      IDbUserMapper dbUserMapper,
      IDbUserAvatarMapper dbUserAvatarMapper,
      IAccessValidator accessValidator,
      IGeneratePasswordCommand generatePassword,
      IResponseCreator responseCreator,
      ITextTemplateParser parser,
      IImageService imageService,
      IEmailService emailService,
      ITextTemplateService textTemplateService,
      IBus bus)
    {
      _validator = validator;
      _userRepository = userRepository;
      _pendingUserRepository = pendingUserRepository;
      _imageRepository = imageRepository;
      _dbUserMapper = dbUserMapper;
      _dbUserAvatarMapper = dbUserAvatarMapper;
      _accessValidator = accessValidator;
      _generatePassword = generatePassword;
      _httpContextAccessor = httpContextAccessor;
      _responseCreator = responseCreator;
      _parser = parser;
      _imageService = imageService;
      _emailService = emailService;
      _textTemplateService = textTemplateService;
      _bus = bus;
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

      await _pendingUserRepository
        .CreateAsync(new DbPendingUser()
        {
          UserId = userId,
          Password = password,
          CommunicationId = dbUser.Communications.FirstOrDefault().Id
        });

      Guid? avatarImageId = request.AvatarImage is null
        ? null
        : await _imageService.CreateImageAsync(request.AvatarImage, response.Errors);

      if (avatarImageId.HasValue)
      {
        await _imageRepository.CreateAsync(_dbUserAvatarMapper.Map(avatarImageId.Value, userId, true));
      }

      await Task.WhenAll(
        NotifyAsync(dbUser, password, "ru", response.Errors),

        request.OfficeId.HasValue
          ? _bus.Publish<ICreateUserOfficePublish>(ICreateUserOfficePublish.CreateObj(
            userId: userId,
            officeId: request.OfficeId.Value,
            createdBy: _httpContextAccessor.HttpContext.GetUserId()))
          : Task.CompletedTask,

        request.RoleId.HasValue
          ? _bus.Publish<ICreateUserRolePublish>(ICreateUserRolePublish.CreateObj(
            roleId: request.RoleId.Value,
            userId: userId,
            changedBy: _httpContextAccessor.HttpContext.GetUserId()))
          : Task.CompletedTask,

        request.DepartmentId.HasValue
          ? _bus.Publish<ICreateDepartmentEntityPublish>(ICreateDepartmentEntityPublish.CreateObj(
            departmentId: request.DepartmentId.Value,
            createdBy: _httpContextAccessor.HttpContext.GetUserId(),
            userId: userId))
          : Task.CompletedTask,

        request.PositionId.HasValue
          ? _bus.Publish<ICreateUserPositionPublish>(ICreateUserPositionPublish.CreateObj(
            positionId: request.PositionId.Value,
            createdBy: _httpContextAccessor.HttpContext.GetUserId(),
            userId: userId))
          : Task.CompletedTask,

        request.UserCompany is not null
          ? _bus.Publish<ICreateCompanyUserPublish>(ICreateCompanyUserPublish.CreateObj(
            companyId: request.UserCompany.CompanyId,
            userId: userId,
            contractSubjectId: request.UserCompany.ContractSubjectId,
            contractTermType: request.UserCompany.ContractTermType,
            rate: request.UserCompany.Rate,
            startWorkingAt: request.UserCompany.StartWorkingAt,
            endWorkingAt: request.UserCompany.EndWorkingAt,
            probation: request.UserCompany.Probation,
            createdBy: _httpContextAccessor.HttpContext.GetUserId()))
          : Task.CompletedTask);

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      response.Body = userId;

      return response;
    }
  }
}