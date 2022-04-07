using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Helpers.TextHandlers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
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
    private readonly ICompanyService _companyService;
    private readonly IDepartmentService _departmentService;
    private readonly IImageService _imageService;
    private readonly IOfficeService _officeService;
    private readonly IPositionService _positionService;
    private readonly IRightService _rightService;
    private readonly ITextTemplateService _textTemplateService;
    private readonly IEmailService _emailService;

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
      ICompanyService companyService,
      IDepartmentService departmentService,
      IImageService imageService,
      IOfficeService officeService,
      IPositionService positionService,
      IRightService rightService,
      ITextTemplateService textTemplateService,
      IEmailService emailService)
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
      _companyService = companyService;
      _departmentService = departmentService;
      _imageService = imageService;
      _officeService = officeService;
      _positionService = positionService;
      _rightService = rightService;
      _textTemplateService = textTemplateService;
      _emailService = emailService;
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
          ? _officeService.CreateUserOfficeAsync(request.OfficeId.Value, userId, response.Errors)
          : Task.FromResult<object>(null),
        request.RoleId.HasValue
          ? _rightService.CreateUserRoleAsync(request.RoleId.Value, userId, response.Errors)
          : Task.FromResult<object>(null),
        request.DepartmentId.HasValue
          ? _departmentService.CreateDepartmentUserAsync(request.DepartmentId.Value, userId, response.Errors)
          : Task.FromResult<object>(null),
        request.PositionId.HasValue
          ? _positionService.CreateUserPositionAsync(request.PositionId.Value, userId, response.Errors)
          : Task.FromResult<object>(null),
        request.UserCompany is not null
          ? _companyService.CreateUserCompanyAsync(request.UserCompany, userId, response.Errors)
          : Task.FromResult<object>(null));

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;
      response.Body = userId;
      response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}