using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Helpers.TextHandlers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Responses.TextTemplate;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Broker.Publishes.Interfaces;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using LT.DigitalOffice.UserService.Business.Commands.User;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Avatar;
using LT.DigitalOffice.UserService.Models.Dto.Requests.UserCompany;
using LT.DigitalOffice.UserService.Validation.User.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.UnitTests.User
{
  class CreateUserCommandTests
  {
    private AutoMocker _mocker;
    private ICreateUserCommand _command;

    private CreateUserRequest _request;
    private OperationResultResponse<Guid> _response;
    private OperationResultResponse<Guid> _failureResponse;
    private DbUser _dbUser;
    private DbUserAvatar _dbUserAvatar;

    private void Verifiable(
      Times accessValidatorCalls,
      Times responseCreatorCalls,
      Times validatorCalls,
      Times mapperCalls,
      Times pendingRepositoryCalls,
      Times imageSericeCalls,
      Times imageMapperCalls,
      Times avaterRepositoryCalls,
      Times templateServiceCalls,
      Times emailServiceCalls,
      Times publishOfficeCalls,
      Times publishRoleCalls,
      Times publishDepartmentCalls,
      Times publishPositionCalls,
      Times publishCompanyCalls)
    {
      _mocker.Verify<IAccessValidator>(
        x => x.HasRightsAsync(It.IsAny<int[]>()),
        accessValidatorCalls);

      _mocker.Verify<IResponseCreator>(
        x => x.CreateFailureResponse<Guid>(It.IsAny<HttpStatusCode>(), It.IsAny<List<string>>()),
        responseCreatorCalls);

      _mocker.Verify<ICreateUserRequestValidator>(
        x => x.ValidateAsync(It.IsAny<CreateUserRequest>(), default),
        validatorCalls);

      _mocker.Verify<IDbUserMapper>(
        x => x.Map(It.IsAny<CreateUserRequest>()),
        mapperCalls);

      _mocker.Verify<IPendingUserRepository>(
        x => x.CreateAsync(It.IsAny<DbPendingUser>()),
        pendingRepositoryCalls);

      _mocker.Verify<IImageService>(
        x => x.CreateImageAsync(It.IsAny<CreateAvatarRequest>(), It.IsAny<List<string>>()),
        imageSericeCalls);

      _mocker.Verify<IDbUserAvatarMapper>(
        x => x.Map(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<bool>()),
        imageMapperCalls);

      _mocker.Verify<IUserAvatarRepository>(
        x => x.CreateAsync(It.IsAny<DbUserAvatar>()),
        avaterRepositoryCalls);

      _mocker.Verify<ITextTemplateService>(
        x => x.GetAsync(It.IsAny<TemplateType>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Guid?>()),
        templateServiceCalls);

      _mocker.Verify<IEmailService>(
        x => x.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()),
        emailServiceCalls);

      _mocker.Verify<IPublish>(
        x => x.CreateUserOfficeAsync(It.IsAny<Guid>(), It.IsAny<Guid>()),
        publishOfficeCalls);

      _mocker.Verify<IPublish>(
        x => x.CreateUserRoleAsync(It.IsAny<Guid>(), It.IsAny<Guid>()),
        publishRoleCalls);

      _mocker.Verify<IPublish>(
        x => x.CreateDepartmentUserAsync(It.IsAny<Guid>(), It.IsAny<Guid>()),
        publishDepartmentCalls);

      _mocker.Verify<IPublish>(
        x => x.CreateUserPositionAsync(It.IsAny<Guid>(), It.IsAny<Guid>()),
        publishPositionCalls);

      _mocker.Verify<IPublish>(
        x => x.CreateCompanyUserAsync(It.IsAny<Guid>(), It.IsAny<CreateUserCompanyRequest>()),
        publishCompanyCalls);

      _mocker.Verify<IPublish>(
        x => x.CreateCompanyUserAsync(It.IsAny<Guid>(), It.IsAny<CreateUserCompanyRequest>()),
        publishCompanyCalls);

      _mocker.Resolvers.Clear();
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _failureResponse = new()
      {
        Body = Guid.Empty,
        Errors = new() { "Error" }
      };

      _dbUser = new()
      {
        Id = Guid.NewGuid(),
        Communications = new List<DbUserCommunication>()
        {
          new DbUserCommunication()
        }
      };

      _dbUserAvatar = new()
      {
        Id = Guid.Empty,
        UserId = _dbUser.Id,
        AvatarId = Guid.NewGuid()
      };

      _response = new(body: _dbUser.Id);
    }

    [SetUp]
    public void SetUp()
    {
      _request = new CreateUserRequest()
      {
        DepartmentId = Guid.Empty,
        OfficeId = Guid.Empty,
        PositionId = Guid.Empty,
        RoleId = Guid.Empty,
        UserCompany = new(),
        AvatarImage = new(),
        Communication = new()
      };

      _mocker = new AutoMocker();

      _mocker
        .Setup<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(It.IsAny<int[]>()))
        .Returns(Task.FromResult(true));

      _mocker
        .Setup<IResponseCreator, OperationResultResponse<Guid>>(x => x.CreateFailureResponse<Guid>(It.IsAny<HttpStatusCode>(), It.IsAny<List<string>>()))
        .Returns(_failureResponse);

      _mocker
        .Setup<ICreateUserRequestValidator, Task<ValidationResult>>(x => x.ValidateAsync(It.IsAny<CreateUserRequest>(), default))
        .Returns(Task.FromResult(new ValidationResult() { }));

      _mocker
        .Setup<IDbUserMapper, DbUser>(x => x.Map(It.IsAny<CreateUserRequest>()))
        .Returns(_dbUser);

      _mocker
        .Setup<IPendingUserRepository, Task>(x => x.CreateAsync(It.IsAny<DbPendingUser>()))
        .Returns(Task.CompletedTask);

      _mocker
        .Setup<IImageService, Task<Guid?>>(x => x.CreateImageAsync(It.IsAny<CreateAvatarRequest>(), It.IsAny<List<string>>()))
        .Returns(Task.FromResult((Guid?)_dbUserAvatar.AvatarId));

      _mocker
        .Setup<IDbUserAvatarMapper, DbUserAvatar>(x => x.Map(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<bool>()))
        .Returns(_dbUserAvatar);

      _mocker
        .Setup<IUserAvatarRepository, Task>(x => x.CreateAsync(It.IsAny<DbUserAvatar>()))
        .Returns(Task.CompletedTask);

      _mocker
        .Setup<IGetTextTemplateResponse, string>(x => x.Text)
        .Returns(string.Empty);

      _mocker
        .Setup<IGetTextTemplateResponse, string>(x => x.Subject)
        .Returns(string.Empty);

      _mocker
        .Setup<ITextTemplateParser, string>(x => x.ParseModel(It.IsAny<Dictionary<string, string>>(), It.IsAny<string>()))
        .Returns(_mocker.GetMock<IGetTextTemplateResponse>().Object.Text);

      _mocker
        .Setup<ITextTemplateParser, string>(x => x.Parse(It.IsAny<Dictionary<string, string>>(), It.IsAny<string>()))
        .Returns(_mocker.GetMock<IGetTextTemplateResponse>().Object.Text);

      _mocker
        .Setup<ITextTemplateService, Task<IGetTextTemplateResponse>>(x => x
          .GetAsync(It.IsAny<TemplateType>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Guid?>()))
        .Returns(Task.FromResult(_mocker.GetMock<IGetTextTemplateResponse>().Object));

      _mocker
        .Setup<IEmailService, Task>(x => x.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
        .Returns(Task.CompletedTask);

      _mocker
        .Setup<IPublish, Task>(x => x.CreateUserOfficeAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
        .Returns(Task.CompletedTask);

      _mocker
        .Setup<IPublish, Task>(x => x.CreateUserRoleAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
        .Returns(Task.CompletedTask);

      _mocker
        .Setup<IPublish, Task>(x => x.CreateDepartmentUserAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
        .Returns(Task.CompletedTask);

      _mocker
        .Setup<IPublish, Task>(x => x.CreateUserPositionAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
        .Returns(Task.CompletedTask);

      _mocker
        .Setup<IPublish, Task>(x => x.CreateCompanyUserAsync(It.IsAny<Guid>(), It.IsAny<CreateUserCompanyRequest>()))
        .Returns(Task.CompletedTask);

      _mocker
        .Setup<IHttpContextAccessor, int>(x => x.HttpContext.Response.StatusCode)
        .Returns(201);

      _command = _mocker.CreateInstance<CreateUserCommand>();
    }

    [Test]
    public void CreateFullUserInfoSuccesTest()
    {
      SerializerAssert.AreEqual(_response, _command.ExecuteAsync(_request).Result);

      Verifiable(
        accessValidatorCalls: Times.Once(),
        responseCreatorCalls: Times.Never(),
        validatorCalls: Times.Once(),
        mapperCalls: Times.Once(),
        pendingRepositoryCalls: Times.Once(),
        imageSericeCalls: Times.Once(),
        imageMapperCalls: Times.Once(),
        avaterRepositoryCalls: Times.Once(),
        templateServiceCalls: Times.Once(),
        emailServiceCalls: Times.Once(),
        publishOfficeCalls: Times.Once(),
        publishRoleCalls: Times.Once(),
        publishDepartmentCalls: Times.Once(),
        publishPositionCalls: Times.Once(),
        publishCompanyCalls: Times.Once());
    }

    [Test]
    public void CreateUserWithoutAvatarSuccesTest()
    {
      _request.AvatarImage = null;

      SerializerAssert.AreEqual(_response, _command.ExecuteAsync(_request).Result);

      Verifiable(
        accessValidatorCalls: Times.Once(),
        responseCreatorCalls: Times.Never(),
        validatorCalls: Times.Once(),
        mapperCalls: Times.Once(),
        pendingRepositoryCalls: Times.Once(),
        imageSericeCalls: Times.Never(),
        imageMapperCalls: Times.Never(),
        avaterRepositoryCalls: Times.Never(),
        templateServiceCalls: Times.Once(),
        emailServiceCalls: Times.Once(),
        publishOfficeCalls: Times.Once(),
        publishRoleCalls: Times.Once(),
        publishDepartmentCalls: Times.Once(),
        publishPositionCalls: Times.Once(),
        publishCompanyCalls: Times.Once());
    }

    [Test]
    public void CreateUserWithoutAvatarIfImageServiceNotRespondedSuccesTest()
    {
      _mocker
        .Setup<IImageService, Task<Guid?>>(x => x.CreateImageAsync(It.IsAny<CreateAvatarRequest>(), It.IsAny<List<string>>()))
        .Returns(Task.FromResult((Guid?)null));

      SerializerAssert.AreEqual(_response, _command.ExecuteAsync(_request).Result);

      Verifiable(
        accessValidatorCalls: Times.Once(),
        responseCreatorCalls: Times.Never(),
        validatorCalls: Times.Once(),
        mapperCalls: Times.Once(),
        pendingRepositoryCalls: Times.Once(),
        imageSericeCalls: Times.Once(),
        imageMapperCalls: Times.Never(),
        avaterRepositoryCalls: Times.Never(),
        templateServiceCalls: Times.Once(),
        emailServiceCalls: Times.Once(),
        publishOfficeCalls: Times.Once(),
        publishRoleCalls: Times.Once(),
        publishDepartmentCalls: Times.Once(),
        publishPositionCalls: Times.Once(),
        publishCompanyCalls: Times.Once());
    }

    [Test]
    public void CreateUserWithoutOfficeSuccesTest()
    {
      _request.OfficeId = null;

      SerializerAssert.AreEqual(_response, _command.ExecuteAsync(_request).Result);

      Verifiable(
        accessValidatorCalls: Times.Once(),
        responseCreatorCalls: Times.Never(),
        validatorCalls: Times.Once(),
        mapperCalls: Times.Once(),
        pendingRepositoryCalls: Times.Once(),
        imageSericeCalls: Times.Once(),
        imageMapperCalls: Times.Once(),
        avaterRepositoryCalls: Times.Once(),
        templateServiceCalls: Times.Once(),
        emailServiceCalls: Times.Once(),
        publishOfficeCalls: Times.Never(),
        publishRoleCalls: Times.Once(),
        publishDepartmentCalls: Times.Once(),
        publishPositionCalls: Times.Once(),
        publishCompanyCalls: Times.Once());
    }

    [Test]
    public void CreateUserWithoutRoleSuccesTest()
    {
      _request.RoleId = null;

      SerializerAssert.AreEqual(_response, _command.ExecuteAsync(_request).Result);

      Verifiable(
        accessValidatorCalls: Times.Once(),
        responseCreatorCalls: Times.Never(),
        validatorCalls: Times.Once(),
        mapperCalls: Times.Once(),
        pendingRepositoryCalls: Times.Once(),
        imageSericeCalls: Times.Once(),
        imageMapperCalls: Times.Once(),
        avaterRepositoryCalls: Times.Once(),
        templateServiceCalls: Times.Once(),
        emailServiceCalls: Times.Once(),
        publishOfficeCalls: Times.Once(),
        publishRoleCalls: Times.Never(),
        publishDepartmentCalls: Times.Once(),
        publishPositionCalls: Times.Once(),
        publishCompanyCalls: Times.Once());
    }

    [Test]
    public void CreateUserWithoutDepartmentSuccesTest()
    {
      _request.DepartmentId = null;

      SerializerAssert.AreEqual(_response, _command.ExecuteAsync(_request).Result);

      Verifiable(
        accessValidatorCalls: Times.Once(),
        responseCreatorCalls: Times.Never(),
        validatorCalls: Times.Once(),
        mapperCalls: Times.Once(),
        pendingRepositoryCalls: Times.Once(),
        imageSericeCalls: Times.Once(),
        imageMapperCalls: Times.Once(),
        avaterRepositoryCalls: Times.Once(),
        templateServiceCalls: Times.Once(),
        emailServiceCalls: Times.Once(),
        publishOfficeCalls: Times.Once(),
        publishRoleCalls: Times.Once(),
        publishDepartmentCalls: Times.Never(),
        publishPositionCalls: Times.Once(),
        publishCompanyCalls: Times.Once());
    }

    [Test]
    public void CreateUserWithoutPositionSuccesTest()
    {
      _request.PositionId = null;

      SerializerAssert.AreEqual(_response, _command.ExecuteAsync(_request).Result);

      Verifiable(
        accessValidatorCalls: Times.Once(),
        responseCreatorCalls: Times.Never(),
        validatorCalls: Times.Once(),
        mapperCalls: Times.Once(),
        pendingRepositoryCalls: Times.Once(),
        imageSericeCalls: Times.Once(),
        imageMapperCalls: Times.Once(),
        avaterRepositoryCalls: Times.Once(),
        templateServiceCalls: Times.Once(),
        emailServiceCalls: Times.Once(),
        publishOfficeCalls: Times.Once(),
        publishRoleCalls: Times.Once(),
        publishDepartmentCalls: Times.Once(),
        publishPositionCalls: Times.Never(),
        publishCompanyCalls: Times.Once());
    }

    [Test]
    public void CreateUserWithoutCompanySuccesTest()
    {
      _request.UserCompany = null;

      SerializerAssert.AreEqual(_response, _command.ExecuteAsync(_request).Result);

      Verifiable(
        accessValidatorCalls: Times.Once(),
        responseCreatorCalls: Times.Never(),
        validatorCalls: Times.Once(),
        mapperCalls: Times.Once(),
        pendingRepositoryCalls: Times.Once(),
        imageSericeCalls: Times.Once(),
        imageMapperCalls: Times.Once(),
        avaterRepositoryCalls: Times.Once(),
        templateServiceCalls: Times.Once(),
        emailServiceCalls: Times.Once(),
        publishOfficeCalls: Times.Once(),
        publishRoleCalls: Times.Once(),
        publishDepartmentCalls: Times.Once(),
        publishPositionCalls: Times.Once(),
        publishCompanyCalls: Times.Never());
    }

    [Test]
    public void CreateUserWithoutTextTemplateSuccesTest()
    {
      _mocker
        .Setup<ITextTemplateService, Task<IGetTextTemplateResponse>>(x => x
          .GetAsync(It.IsAny<TemplateType>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Guid?>()))
        .Returns(Task.FromResult((IGetTextTemplateResponse)null));

      SerializerAssert.AreEqual(_response, _command.ExecuteAsync(_request).Result);

      Verifiable(
        accessValidatorCalls: Times.Once(),
        responseCreatorCalls: Times.Never(),
        validatorCalls: Times.Once(),
        mapperCalls: Times.Once(),
        pendingRepositoryCalls: Times.Once(),
        imageSericeCalls: Times.Once(),
        imageMapperCalls: Times.Once(),
        avaterRepositoryCalls: Times.Once(),
        templateServiceCalls: Times.Once(),
        emailServiceCalls: Times.Never(),
        publishOfficeCalls: Times.Once(),
        publishRoleCalls: Times.Once(),
        publishDepartmentCalls: Times.Once(),
        publishPositionCalls: Times.Once(),
        publishCompanyCalls: Times.Once());
    }

    [Test]
    public void NotEnoughRightTest()
    {
      _mocker
        .Setup<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(It.IsAny<int[]>()))
        .Returns(Task.FromResult(false));

      SerializerAssert.AreEqual(_failureResponse, _command.ExecuteAsync(_request).Result);

      Verifiable(
        accessValidatorCalls: Times.Once(),
        responseCreatorCalls: Times.Once(),
        validatorCalls: Times.Never(),
        mapperCalls: Times.Never(),
        pendingRepositoryCalls: Times.Never(),
        imageSericeCalls: Times.Never(),
        imageMapperCalls: Times.Never(),
        avaterRepositoryCalls: Times.Never(),
        templateServiceCalls: Times.Never(),
        emailServiceCalls: Times.Never(),
        publishOfficeCalls: Times.Never(),
        publishRoleCalls: Times.Never(),
        publishDepartmentCalls: Times.Never(),
        publishPositionCalls: Times.Never(),
        publishCompanyCalls: Times.Never());
    }

    [Test]
    public void ValidationFailureTest()
    {
      _mocker
        .Setup<ICreateUserRequestValidator, Task<ValidationResult>>(x => x.ValidateAsync(It.IsAny<CreateUserRequest>(), default))
        .Returns(Task.FromResult(new ValidationResult(new List<ValidationFailure>() { new ValidationFailure("_", "Error") })));

      SerializerAssert.AreEqual(_failureResponse, _command.ExecuteAsync(_request).Result);

      Verifiable(
        accessValidatorCalls: Times.Once(),
        responseCreatorCalls: Times.Once(),
        validatorCalls: Times.Once(),
        mapperCalls: Times.Never(),
        pendingRepositoryCalls: Times.Never(),
        imageSericeCalls: Times.Never(),
        imageMapperCalls: Times.Never(),
        avaterRepositoryCalls: Times.Never(),
        templateServiceCalls: Times.Never(),
        emailServiceCalls: Times.Never(),
        publishOfficeCalls: Times.Never(),
        publishRoleCalls: Times.Never(),
        publishDepartmentCalls: Times.Never(),
        publishPositionCalls: Times.Never(),
        publishCompanyCalls: Times.Never());
    }
  }
}
