using FluentValidation.Results;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Responses.Auth;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using LT.DigitalOffice.UserService.Business.Commands.Credentials;
using LT.DigitalOffice.UserService.Business.Commands.Credentials.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Credentials;
using LT.DigitalOffice.UserService.Validation.Credentials.Interfaces;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.UnitTests.Credentials
{
  class CreateCredentialsCommandTests
  {
    private AutoMocker _mocker;
    private ICreateCredentialsCommand _command;

    private Guid _userId = Guid.NewGuid();
    private Guid? _userCredentialsId = Guid.NewGuid();
    private string _accessToken = String.Empty;
    private string _refreshToken = String.Empty;
    private double _accessTokenExpiresIn = default;
    private double _refreshTokenExpiresIn = default;
    private string errorMessage = "Error message";

    private DbPendingUser _dbPendingUser = new();
    private DbUserCredentials _dbUserCredentials = new();
    private CreateCredentialsRequest _request;
    private OperationResultResponse<CredentialsResponse> _badRequestResponse = new();
    private OperationResultResponse<CredentialsResponse> _response = new();

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _request = new()
      {
        UserId = _userId,
        Password = "Password",
        Login = "Login"
      };

      _badRequestResponse = new(
        body: default,
        status: OperationResultStatusType.Failed,
        errors: new List<string>() { errorMessage });
    }

    [SetUp]
    public void SetUp()
    {
      _mocker = new AutoMocker();

      _mocker
        .Setup<ICreateCredentialsRequestValidator, Task<ValidationResult>>(x => x.ValidateAsync(It.IsAny<CreateCredentialsRequest>(), default))
        .Returns(Task.FromResult(new ValidationResult() { }));

      _mocker
        .Setup<IResponseCreator, OperationResultResponse<CredentialsResponse>>(x =>
          x.CreateFailureResponse<CredentialsResponse>(It.IsAny<HttpStatusCode>(), It.IsAny<List<string>>()))
        .Returns(_badRequestResponse);

      _mocker
        .Setup<IGetTokenResponse, string>(x => x.AccessToken)
        .Returns(_accessToken);

      _mocker
        .Setup<IGetTokenResponse, string>(x => x.RefreshToken)
        .Returns(_refreshToken);

      _mocker
        .Setup<IGetTokenResponse, double>(x => x.AccessTokenExpiresIn)
        .Returns(_accessTokenExpiresIn);

      _mocker
        .Setup<IGetTokenResponse, double>(x => x.RefreshTokenExpiresIn)
        .Returns(_refreshTokenExpiresIn);

      _mocker
        .Setup<IAuthService, Task<IGetTokenResponse>>(x => x.GetTokenAsync(It.IsAny<Guid>(), It.IsAny<List<string>>()))
        .Returns(Task.FromResult(_mocker.GetMock<IGetTokenResponse>().Object));

      _mocker
        .Setup<IUserCredentialsRepository, Task<Guid?>>(x => x.CreateAsync(It.IsAny<DbUserCredentials>()))
        .Returns(Task.FromResult(_userCredentialsId));

      _mocker
        .Setup<IDbUserCredentialsMapper, DbUserCredentials>(x => x.Map(It.IsAny<CreateCredentialsRequest>(), It.IsAny<string>(), It.IsAny<string>()))
        .Returns(_dbUserCredentials);

      _mocker
        .Setup<IPendingUserRepository, Task<DbPendingUser>>(x => x.RemoveAsync(It.IsAny<Guid>()))
        .Returns(Task.FromResult(_dbPendingUser));

      _mocker
        .Setup<IUserRepository, Task<bool>>(x => x.SwitchActiveStatusAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
        .Returns(Task.FromResult(true));

      _mocker
        .Setup<IUserCommunicationRepository, Task>(x => x.SetBaseTypeAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
        .Returns(Task.CompletedTask);

      _command = _mocker.CreateInstance<CreateCredentialsCommand>();
    }

    [Test]
    public void SuccessTest()
    {
      _response.Body = new()
      {
        UserId = _userId,
        AccessToken = _accessToken,
        RefreshToken = _refreshToken,
        AccessTokenExpiresIn = _refreshTokenExpiresIn,
        RefreshTokenExpiresIn = _refreshTokenExpiresIn
      };

      SerializerAssert.AreEqual(_response, _command.ExecuteAsync(_request).Result);

      Verifiable(
        validatorCalls: Times.Once(),
        responseCreatorCalls: Times.Never(),
        authServiceCalls: Times.Once(),
        userCredentialsRepositoryCalls: Times.Once(),
        dbUserCredentialsMapperCalls: Times.Once(),
        pendingUserRepositoryCalls: Times.Once(),
        userRepositoryCalls: Times.Once(),
        userCommunicationRepositoryCalls: Times.Once());
    }

    [Test]
    public void FailedValidation()
    {
      _mocker
        .Setup<ICreateCredentialsRequestValidator, Task<ValidationResult>>(x => x.ValidateAsync(It.IsAny<CreateCredentialsRequest>(), default))
        .Returns(Task.FromResult(new ValidationResult(new List<ValidationFailure>() { new ValidationFailure("_", errorMessage) })));

      SerializerAssert.AreEqual(_badRequestResponse, _command.ExecuteAsync(_request).Result);

      Verifiable(
        validatorCalls: Times.Once(),
        responseCreatorCalls: Times.Once(),
        authServiceCalls: Times.Never(),
        userCredentialsRepositoryCalls: Times.Never(),
        dbUserCredentialsMapperCalls: Times.Never(),
        pendingUserRepositoryCalls: Times.Never(),
        userRepositoryCalls: Times.Never(),
        userCommunicationRepositoryCalls: Times.Never());
    }

    [Test]
    public void NullTokenResponse()
    {
      IGetTokenResponse tokenResponse = null;

      _mocker
        .Setup<IAuthService, Task<IGetTokenResponse>>(x => x.GetTokenAsync(It.IsAny<Guid>(), It.IsAny<List<string>>()))
        .Returns(Task.FromResult(tokenResponse));

      SerializerAssert.AreEqual(_badRequestResponse, _command.ExecuteAsync(_request).Result);

      Verifiable(
        validatorCalls: Times.Once(),
        responseCreatorCalls: Times.Once(),
        authServiceCalls: Times.Once(),
        userCredentialsRepositoryCalls: Times.Never(),
        dbUserCredentialsMapperCalls: Times.Never(),
        pendingUserRepositoryCalls: Times.Never(),
        userRepositoryCalls: Times.Never(),
        userCommunicationRepositoryCalls: Times.Never());
    }

    private void Verifiable(
      Times validatorCalls,
      Times responseCreatorCalls,
      Times authServiceCalls,
      Times userCredentialsRepositoryCalls,
      Times dbUserCredentialsMapperCalls,
      Times pendingUserRepositoryCalls,
      Times userRepositoryCalls,
      Times userCommunicationRepositoryCalls)
    {
      _mocker.Verify<ICreateCredentialsRequestValidator>(
        x => x.ValidateAsync(It.IsAny<CreateCredentialsRequest>(), default),
        validatorCalls);

      _mocker.Verify<IResponseCreator>(
        x => x.CreateFailureResponse<CredentialsResponse>(It.IsAny<HttpStatusCode>(), It.IsAny<List<string>>()),
        responseCreatorCalls);

      _mocker.Verify<IAuthService>(
        x => x.GetTokenAsync(It.IsAny<Guid>(), It.IsAny<List<string>>()),
        authServiceCalls);

      _mocker.Verify<IUserCredentialsRepository>(
        x => x.CreateAsync(It.IsAny<DbUserCredentials>()),
        userCredentialsRepositoryCalls);

      _mocker.Verify<IDbUserCredentialsMapper>(
        x => x.Map(It.IsAny<CreateCredentialsRequest>(), It.IsAny<string>(), It.IsAny<string>()),
        dbUserCredentialsMapperCalls);

      _mocker.Verify<IPendingUserRepository>(
        x => x.RemoveAsync(It.IsAny<Guid>()),
        pendingUserRepositoryCalls);

      _mocker.Verify<IUserRepository>(
        x => x.SwitchActiveStatusAsync(It.IsAny<Guid>(), It.IsAny<bool>()),
        userRepositoryCalls);

      _mocker.Verify<IUserCommunicationRepository>(
        x => x.SetBaseTypeAsync(It.IsAny<Guid>(), It.IsAny<Guid>()),
        userCommunicationRepositoryCalls);

      _mocker.Resolvers.Clear();
    }
  }
}
