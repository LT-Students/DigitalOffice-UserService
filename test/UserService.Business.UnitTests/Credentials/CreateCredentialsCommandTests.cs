using FluentValidation.Results;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Responses.Auth;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using LT.DigitalOffice.UserService.Business.Commands.Credentials;
using LT.DigitalOffice.UserService.Business.Commands.Credentials.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Credentials;
using LT.DigitalOffice.UserService.Validation.Credentials.Interfaces;
using Microsoft.Extensions.Logging;
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

    private CreateCredentialsRequest _request;
    private IGetTokenResponse _tokenResponse;
    private Guid _userId = Guid.NewGuid();
    private string _accessToken = String.Empty;
    private string _refreshToken = String.Empty;
    private double _accessTokenExpiresIn = default;
    private double _refreshTokenExpiresIn = default;

    private DbPendingUser _dbPendingUser;

    
    private string _password = "password";
    private CredentialsResponse _response;


    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _mocker
        .Setup<ICreateCredentialsRequestValidator, Task<ValidationResult>>(x => x.ValidateAsync(It.IsAny<CreateCredentialsRequest>(), default))
        .Returns(Task.FromResult(new ValidationResult()));

      _mocker
        .Setup<IResponseCreator, OperationResultResponse<CredentialsResponse>>(x =>
          x.CreateFailureResponse<CredentialsResponse>(It.IsAny<HttpStatusCode>(), default))
        .Returns(new OperationResultResponse<CredentialsResponse>());

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
    }

    /*[SetUp]
    public void SetUp()
    {
        _dbPendingUser = new()
      {
           UserId = _userId,
           Password = _password
       };

       _request = new CreateCredentialsRequest()
       {
           UserId = _userId,
           Login = "login",
           Password = _password
        };

       _userAccessToken = "Access";
       _userRefreshToken = "Refresh";

    //    _response = new()
    //    {
    //        UserId = _userId,
    //        AccessToken = _userAccessToken,
    //        RefreshToken = _userRefreshToken,
    //        AccessTokenExpiresIn = 100,
    //        RefreshTokenExpiresIn = 250
    //    };

    //    _loggerMock = new Mock<ILogger<CreateCredentialsCommand>>();

    //    _mocker = new AutoMocker();

    //    _mocker
    //        .Setup<IUserRepository, DbPendingUser>(
    //            x => x.GetPendingUser(_userId))
    //        .Returns(_dbPendingUser);

    //    _mocker
    //        .Setup<IUserRepository>(
    //            x => x.DeletePendingUser(It.IsAny<Guid>()));

    //    _mocker
    //        .Setup<IUserRepository, bool>(
    //            x => x.SwitchActiveStatus(It.IsAny<Guid>(), true))
    //        .Returns(true);

    //    _mocker
    //        .Setup<IDbUserCredentialsMapper, DbUserCredentials>(
    //            x => x.Map(It.IsAny<CreateCredentialsRequest>(),
    //                It.IsAny<string>(),
    //                It.IsAny<string>()))
    //        .Returns(new DbUserCredentials());

    //    _mocker
    //        .Setup<IUserCredentialsRepository, Guid>(
    //            x => x.Create(It.IsAny<DbUserCredentials>()))
    //        .Returns(new Guid());

    //    var getTokenResponseMock = new Mock<IGetTokenResponse>();
    //    getTokenResponseMock.Setup(x => x.AccessToken).Returns("Access");
    //    getTokenResponseMock.Setup(x => x.RefreshToken).Returns("Refresh");
    //    getTokenResponseMock.Setup(x => x.AccessTokenExpiresIn).Returns(100);
    //    getTokenResponseMock.Setup(x => x.RefreshTokenExpiresIn).Returns(250);

    //    _mocker
    //        .Setup<IOperationResult<IGetTokenResponse>, IGetTokenResponse>(x => x.Body)
    //        .Returns(getTokenResponseMock.Object);
    //    _mocker
    //        .Setup<IOperationResult<IGetTokenResponse>, bool>(x => x.IsSuccess)
    //        .Returns(true);
    //    _mocker
    //        .Setup<IRequestClient<IGetTokenRequest>, IOperationResult<IGetTokenResponse>>(
    //            x => x.GetResponse<IOperationResult<IGetTokenResponse>>(
    //                IGetTokenRequest.CreateObj(_userId),
    //                default,
    //                default)
    //            .Result.Message)
    //        .Returns(_mocker.GetMock<IOperationResult<IGetTokenResponse>>().Object);

    //    ////needs to DI ILoginValidator in CreateCredentialsCommand
    //    /*_command = new CreateCredentialsCommand(
    //        _mocker.GetMock<IDbUserCredentialsMapper>().Object,
    //        _mocker.GetMock<IUserRepository>().Object,
    //        _mocker.GetMock<IUserCredentialsRepository>().Object,
    //        _mocker.GetMock<IRequestClient<IGetTokenRequest>>().Object,
    //        _loggerMock.Object);*/
    //}

    //test fails due to DI ILoginValidator in CreateCredentialsCommand
    /*[Test]
    public void ThrowExсeptionWhenRequestIsNull()
    {
        _request = null;

        Assert.Throws<BadRequestException>(() => _command.Execute(_request));

        _mocker.Verify<IUserRepository, DbPendingUser>(
            x => x.GetPendingUser(It.IsAny<Guid>()),
            Times.Never());

        _mocker.Verify<IDbUserCredentialsMapper, DbUserCredentials>(
            x => x.Map(It.IsAny<CreateCredentialsRequest>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never());

        _mocker.Verify<IRequestClient<IGetTokenRequest>, IOperationResult<IGetTokenResponse>>(
            x => x.GetResponse<IOperationResult<IGetTokenResponse>>(
                IGetTokenRequest.CreateObj(_userId),
                default,
                default)
            .Result.Message,
            Times.Never());

        _mocker.Verify<IUserCredentialsRepository, Guid>(
            x => x.Create(It.IsAny<DbUserCredentials>()),
            Times.Never());

        _mocker.Verify<IUserRepository>(
            x => x.DeletePendingUser(It.IsAny<Guid>()),
            Times.Never());

        _mocker.Verify<IUserRepository, bool>(
            x => x.SwitchActiveStatus(It.IsAny<Guid>(), It.IsAny<bool>()),
            Times.Never());
    }*/

    /*[Test]
    public void ThrowExсeptionWhenDbPendingUserIsNull()
    {
        DbPendingUser dbPendingUser = null;
        _mocker
            .Setup<IUserRepository, DbPendingUser>(
                x => x.GetPendingUser(_userId))
            .Returns(dbPendingUser);

        Assert.Throws<NotFoundException>(() => _command.Execute(_request));

        _mocker.Verify<IUserRepository, DbPendingUser>(
            x => x.GetPendingUser(It.IsAny<Guid>()),
            Times.Once());

        _mocker.Verify<IDbUserCredentialsMapper, DbUserCredentials>(
            x => x.Map(It.IsAny<CreateCredentialsRequest>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never());

        _mocker.Verify<IRequestClient<IGetTokenRequest>, IOperationResult<IGetTokenResponse>>(
            x => x.GetResponse<IOperationResult<IGetTokenResponse>>(
                IGetTokenRequest.CreateObj(_userId),
                default,
                default)
            .Result.Message,
            Times.Never());

        _mocker.Verify<IUserCredentialsRepository, Guid>(
            x => x.Create(It.IsAny<DbUserCredentials>()),
            Times.Never());

        _mocker.Verify<IUserRepository>(
            x => x.DeletePendingUser(It.IsAny<Guid>()),
            Times.Never());

        _mocker.Verify<IUserRepository, bool>(
            x => x.SwitchActiveStatus(It.IsAny<Guid>(), It.IsAny<bool>()),
            Times.Never());
    }*/

    /*[Test]
    public void ThrowExсeptionWhenLoginIsBusyOrCredentialsIsExist()
    {
        Assert.Throws<BadRequestException>(() => _command.Execute(_request));

        _mocker.Verify<IUserRepository, DbPendingUser>(
            x => x.GetPendingUser(It.IsAny<Guid>()),
            Times.Once());

        _mocker.Verify<IDbUserCredentialsMapper, DbUserCredentials>(
            x => x.Map(It.IsAny<CreateCredentialsRequest>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never());

        _mocker.Verify<IRequestClient<IGetTokenRequest>, IOperationResult<IGetTokenResponse>>(
            x => x.GetResponse<IOperationResult<IGetTokenResponse>>(
                IGetTokenRequest.CreateObj(_userId),
                default,
                default)
            .Result.Message,
            Times.Never());

        _mocker.Verify<IUserCredentialsRepository, Guid>(
            x => x.Create(It.IsAny<DbUserCredentials>()),
            Times.Never());

        _mocker.Verify<IUserRepository>(
            x => x.DeletePendingUser(It.IsAny<Guid>()),
            Times.Never());

        _mocker.Verify<IUserRepository, bool>(
            x => x.SwitchActiveStatus(It.IsAny<Guid>(), It.IsAny<bool>()),
            Times.Never());
    }*/

    /*[Test]
    public void ThrowExсeptionWhenPasswordIsNotRight()
    {
        _request.Password = "notRightPassword";

        Assert.Throws<ForbiddenException>(() => _command.Execute(_request));

        _mocker.Verify<IUserRepository, DbPendingUser>(
            x => x.GetPendingUser(It.IsAny<Guid>()),
            Times.Once());

        _mocker.Verify<IDbUserCredentialsMapper, DbUserCredentials>(
            x => x.Map(It.IsAny<CreateCredentialsRequest>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never());

        _mocker.Verify<IRequestClient<IGetTokenRequest>, IOperationResult<IGetTokenResponse>>(
            x => x.GetResponse<IOperationResult<IGetTokenResponse>>(
                IGetTokenRequest.CreateObj(_userId),
                default,
                default)
            .Result.Message,
            Times.Never());

        _mocker.Verify<IUserCredentialsRepository, Guid>(
            x => x.Create(It.IsAny<DbUserCredentials>()),
            Times.Never());

        _mocker.Verify<IUserRepository>(
            x => x.DeletePendingUser(It.IsAny<Guid>()),
            Times.Never());

        _mocker.Verify<IUserRepository, bool>(
            x => x.SwitchActiveStatus(It.IsAny<Guid>(), It.IsAny<bool>()),
            Times.Never());
    }*/

    //test fails due to DI ILoginValidator in CreateCredentialsCommand
    /*[Test]
    public void ThrowExceptionWhenBrokerResponseIsNotSuccess()
    {
        _mocker
            .Setup<IOperationResult<IGetTokenResponse>, bool>(x => x.IsSuccess)
            .Returns(false);

        Assert.Throws<BadRequestException>(() => _command.Execute(_request));

        _mocker.Verify<IUserRepository, DbPendingUser>(
            x => x.GetPendingUser(It.IsAny<Guid>()),
            Times.Once());

        _mocker.Verify<IDbUserCredentialsMapper, DbUserCredentials>(
            x => x.Map(It.IsAny<CreateCredentialsRequest>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never());

        _mocker.Verify<IRequestClient<IGetTokenRequest>, IOperationResult<IGetTokenResponse>>(
            x => x.GetResponse<IOperationResult<IGetTokenResponse>>(
                IGetTokenRequest.CreateObj(_userId),
                default,
                default)
            .Result.Message,
            Times.Once());

        _mocker.Verify<IUserCredentialsRepository, Guid>(
            x => x.Create(It.IsAny<DbUserCredentials>()),
            Times.Never());

        _mocker.Verify<IUserRepository>(
            x => x.DeletePendingUser(It.IsAny<Guid>()),
            Times.Never());

        _mocker.Verify<IUserRepository, bool>(
            x => x.SwitchActiveStatus(It.IsAny<Guid>(), It.IsAny<bool>()),
            Times.Never());
    }
    [Test]
    public void ThrowExсeptionWhenMapperThrowsIt()
    {
        _mocker
            .Setup<IDbUserCredentialsMapper, DbUserCredentials>(
                x => x.Map(It.IsAny<CreateCredentialsRequest>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
            .Throws(new ArgumentNullException());

        Assert.Throws<BadRequestException>(() => _command.Execute(_request));

        _mocker.Verify<IUserRepository, DbPendingUser>(
            x => x.GetPendingUser(It.IsAny<Guid>()),
            Times.Once());

        _mocker.Verify<IRequestClient<IGetTokenRequest>, IOperationResult<IGetTokenResponse>>(
            x => x.GetResponse<IOperationResult<IGetTokenResponse>>(
                IGetTokenRequest.CreateObj(_userId),
                default,
                default)
            .Result.Message,
            Times.Once());

        _mocker.Verify<IDbUserCredentialsMapper, DbUserCredentials>(
            x => x.Map(It.IsAny<CreateCredentialsRequest>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Once());

        _mocker.Verify<IUserCredentialsRepository, Guid>(
            x => x.Create(It.IsAny<DbUserCredentials>()),
            Times.Never());

        _mocker.Verify<IUserRepository>(
            x => x.DeletePendingUser(It.IsAny<Guid>()),
            Times.Never());

        _mocker.Verify<IUserRepository, bool>(
            x => x.SwitchActiveStatus(It.IsAny<Guid>(), It.IsAny<bool>()),
            Times.Never());
    }*/

    //test fails due to DI ILoginValidator in CreateCredentialsCommand
    /*[Test]
    public void ThrowExсeptionWhenUserCredentialsRepositoryThrowsIt()
    {
        _mocker
            .Setup<IUserCredentialsRepository, Guid>(
                x => x.Create(It.IsAny<DbUserCredentials>()))
            .Throws(new Exception());

        Assert.Throws<BadRequestException>(() => _command.Execute(_request));

        _mocker.Verify<IUserRepository, DbPendingUser>(
            x => x.GetPendingUser(It.IsAny<Guid>()),
            Times.Once());

        _mocker.Verify<IRequestClient<IGetTokenRequest>, IOperationResult<IGetTokenResponse>>(
            x => x.GetResponse<IOperationResult<IGetTokenResponse>>(
                IGetTokenRequest.CreateObj(_userId),
                default,
                default)
            .Result.Message,
            Times.Once());

        _mocker.Verify<IDbUserCredentialsMapper, DbUserCredentials>(
            x => x.Map(It.IsAny<CreateCredentialsRequest>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Once());

        _mocker.Verify<IUserCredentialsRepository, Guid>(
            x => x.Create(It.IsAny<DbUserCredentials>()),
            Times.Once());

        _mocker.Verify<IUserRepository>(
            x => x.DeletePendingUser(It.IsAny<Guid>()),
            Times.Never());

        _mocker.Verify<IUserRepository, bool>(
            x => x.SwitchActiveStatus(It.IsAny<Guid>(), It.IsAny<bool>()),
            Times.Never());
    }*/

    //test fails due to DI ILoginValidator in CreateCredentialsCommand
    /*[Test]
    public void ThrowExсeptionWhenUserRepositoryThrowsItWhenDeletePendingUser()
    {
        _mocker
            .Setup<IUserRepository>(
                x => x.DeletePendingUser(It.IsAny<Guid>()))
            .Throws(new Exception());

        Assert.Throws<BadRequestException>(() => _command.Execute(_request));

        _mocker.Verify<IUserRepository, DbPendingUser>(
            x => x.GetPendingUser(It.IsAny<Guid>()),
            Times.Once());

        _mocker.Verify<IRequestClient<IGetTokenRequest>, IOperationResult<IGetTokenResponse>>(
            x => x.GetResponse<IOperationResult<IGetTokenResponse>>(
                IGetTokenRequest.CreateObj(_userId),
                default,
                default)
            .Result.Message,
            Times.Once());

        _mocker.Verify<IDbUserCredentialsMapper, DbUserCredentials>(
            x => x.Map(It.IsAny<CreateCredentialsRequest>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Once());

        _mocker.Verify<IUserCredentialsRepository, Guid>(
            x => x.Create(It.IsAny<DbUserCredentials>()),
            Times.Once());

        _mocker.Verify<IUserRepository>(
            x => x.DeletePendingUser(It.IsAny<Guid>()),
            Times.Once());

        _mocker.Verify<IUserRepository, bool>(
            x => x.SwitchActiveStatus(It.IsAny<Guid>(), It.IsAny<bool>()),
            Times.Never());
    }*/

    //test fails due to DI ILoginValidator in CreateCredentialsCommand
    /*[Test]
    public void ThrowExсeptionWhenUserRepositoryThrowsItWhenSwitchActiveStatus()
    {
        _mocker
            .Setup<IUserRepository, bool>(
                x => x.SwitchActiveStatus(It.IsAny<Guid>(), true))
            .Throws(new Exception());

        Assert.Throws<BadRequestException>(() => _command.Execute(_request));

        _mocker.Verify<IUserRepository, DbPendingUser>(
            x => x.GetPendingUser(It.IsAny<Guid>()),
            Times.Once());

        _mocker.Verify<IRequestClient<IGetTokenRequest>, IOperationResult<IGetTokenResponse>>(
            x => x.GetResponse<IOperationResult<IGetTokenResponse>>(
                IGetTokenRequest.CreateObj(_userId),
                default,
                default)
            .Result.Message,
            Times.Once());

        _mocker.Verify<IDbUserCredentialsMapper, DbUserCredentials>(
            x => x.Map(It.IsAny<CreateCredentialsRequest>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Once());

        _mocker.Verify<IUserCredentialsRepository, Guid>(
            x => x.Create(It.IsAny<DbUserCredentials>()),
            Times.Once());

        _mocker.Verify<IUserRepository>(
            x => x.DeletePendingUser(It.IsAny<Guid>()),
            Times.Once());

        _mocker.Verify<IUserRepository, bool>(
            x => x.SwitchActiveStatus(It.IsAny<Guid>(), It.IsAny<bool>()),
            Times.Once());
    }*/

    /*[Test]
    public void SuccessTest()
    {
        SerializerAssert.AreEqual(_response, _command.Execute(_request));

        _mocker.Verify<IUserRepository, DbPendingUser>(
            x => x.GetPendingUser(It.IsAny<Guid>()),
            Times.Once());

        _mocker.Verify<IUserCredentialsRepository>(
            x => x.CheckLogin(It.IsAny<string>(), It.IsAny<Guid>()),
            Times.Once());

        _mocker.Verify<IRequestClient<IGetTokenRequest>, IOperationResult<IGetTokenResponse>>(
            x => x.GetResponse<IOperationResult<IGetTokenResponse>>(
                IGetTokenRequest.CreateObj(_userId),
                default,
                default)
            .Result.Message,
            Times.Once());

        _mocker.Verify<IDbUserCredentialsMapper, DbUserCredentials>(
            x => x.Map(It.IsAny<CreateCredentialsRequest>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Once());

        _mocker.Verify<IUserCredentialsRepository, Guid>(
            x => x.Create(It.IsAny<DbUserCredentials>()),
            Times.Once());

        _mocker.Verify<IUserRepository>(
            x => x.DeletePendingUser(It.IsAny<Guid>()),
            Times.Once());

        _mocker.Verify<IUserRepository, bool>(
            x => x.SwitchActiveStatus(It.IsAny<Guid>(), It.IsAny<bool>()),
            Times.Once());
    }*/
  }
}
