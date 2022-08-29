using FluentValidation.Results;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Business.Commands.Password;
using LT.DigitalOffice.UserService.Business.Commands.Password.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Helpers.Password;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials.Filters;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Password;
using LT.DigitalOffice.UserService.Validation.Password.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.UnitTests.Password
{
  internal class ChangePasswordCommandTests
  {
    private AutoMocker _mocker;
    private IChangePasswordCommand _command;
    private ChangePasswordRequest _request;
    private OperationResultResponse<bool> _failureResponse;
    private OperationResultResponse<bool> _response;
    private DbUserCredentials _userCredentials;
    private IDictionary<object, object> _items;

    private void Verifiable(
      Times passwordValidatorCalls,
      Times responseCreatorCalls,
      Times userCredentialsRepositoryGetCalls,
      Times userCredentialsRepositoryEditCalls)
    {
      _mocker.Verify<IPasswordValidator>(
        x => x.ValidateAsync(_request.NewPassword, default),
        passwordValidatorCalls);

      _mocker.Verify<IResponseCreator>(
        x => x.CreateFailureResponse<bool>(It.IsAny<HttpStatusCode>(), It.IsAny<List<string>>()),
        responseCreatorCalls);

      _mocker.Verify<IUserCredentialsRepository>(
        x => x.GetAsync(It.IsAny<GetCredentialsFilter>()),
        userCredentialsRepositoryGetCalls);

      _mocker.Verify<IUserCredentialsRepository>(
        x => x.EditAsync(It.IsAny<DbUserCredentials>()),
        userCredentialsRepositoryEditCalls);

      _mocker.Resolvers.Clear();
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _items = new Dictionary<object, object>();
      _items.Add("UserId", Guid.NewGuid());

      _response = new OperationResultResponse<bool>(body: true);

      _failureResponse = new OperationResultResponse<bool>(body: false, errors: new List<string>() { "Error" });

      _userCredentials = new DbUserCredentials()
      {
        Salt = "salt",
        Login = "Login",
      };

      _userCredentials.PasswordHash = UserPasswordHash
        .GetPasswordHash(_userCredentials.Login, _userCredentials.Salt, "Password");
    }

    [SetUp]
    public void SerUp()
    {
      _request = new ChangePasswordRequest()
      {
        Password = "Password",
        NewPassword = "NewPassword"
      };

      _mocker = new AutoMocker();

      _mocker
        .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
        .Returns(_items);

      _mocker
        .Setup<IPasswordValidator, Task<ValidationResult>>(x => x.ValidateAsync(_request.NewPassword, default))
        .Returns(Task.FromResult(new ValidationResult() { }));

      _mocker
        .Setup<IResponseCreator, OperationResultResponse<bool>>(x => x.CreateFailureResponse<bool>(It.IsAny<HttpStatusCode>(), It.IsAny<List<string>>()))
        .Returns(_failureResponse);

      _mocker
        .Setup<IUserCredentialsRepository, Task<DbUserCredentials>>(x => x.GetAsync(It.IsAny<GetCredentialsFilter>()))
        .Returns(Task.FromResult(_userCredentials));

      _mocker
        .Setup<IUserCredentialsRepository, Task<bool>>(x => x.EditAsync(It.IsAny<DbUserCredentials>()))
        .Returns(Task.FromResult(true));

      _command = _mocker.CreateInstance<ChangePasswordCommand>();
    }

    [Test]
    public void SuccessTest()
    {
      SerializerAssert.AreEqual(_response, _command.ExecuteAsync(_request).Result);

      Verifiable(
      passwordValidatorCalls: Times.Once(),
      responseCreatorCalls: Times.Never(),
      userCredentialsRepositoryGetCalls: Times.Once(),
      userCredentialsRepositoryEditCalls: Times.Once());
    }

    [Test]
    public void ValidationFailureTest()
    {
      _mocker
        .Setup<IPasswordValidator, Task<ValidationResult>>(x => x.ValidateAsync(_request.NewPassword, default))
        .Returns(Task.FromResult(new ValidationResult(new List<ValidationFailure>() { new ValidationFailure("_", "Error") })));

      SerializerAssert.AreEqual(_failureResponse, _command.ExecuteAsync(_request).Result);

      Verifiable(
      passwordValidatorCalls: Times.Once(),
      responseCreatorCalls: Times.Once(),
      userCredentialsRepositoryGetCalls: Times.Never(),
      userCredentialsRepositoryEditCalls: Times.Never());
    }

    [Test]
    public void FailureIfDbUserCredentialsNotFoundTest()
    {
      _mocker
        .Setup<IUserCredentialsRepository, Task<DbUserCredentials>>(x => x.GetAsync(It.IsAny<GetCredentialsFilter>()))
        .Returns(Task.FromResult((DbUserCredentials)null));

      SerializerAssert.AreEqual(_failureResponse, _command.ExecuteAsync(_request).Result);

      Verifiable(
      passwordValidatorCalls: Times.Once(),
      responseCreatorCalls: Times.Once(),
      userCredentialsRepositoryGetCalls: Times.Once(),
      userCredentialsRepositoryEditCalls: Times.Never());
    }

    [Test]
    public void FailureIfWrongOldPasswordHashTest()
    {
      _request.Password = "WrongPassword";

      SerializerAssert.AreEqual(_failureResponse, _command.ExecuteAsync(_request).Result);

      Verifiable(
      passwordValidatorCalls: Times.Once(),
      responseCreatorCalls: Times.Once(),
      userCredentialsRepositoryGetCalls: Times.Once(),
      userCredentialsRepositoryEditCalls: Times.Never());
    }
  }
}
