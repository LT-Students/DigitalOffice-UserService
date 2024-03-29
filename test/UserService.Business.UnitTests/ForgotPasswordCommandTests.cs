﻿using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Requests.Email;
using LT.DigitalOffice.UserService.Business.Commands.Password;
using LT.DigitalOffice.UserService.Business.Commands.Password.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Configurations;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.UnitTests
{
  class ForgotPasswordCommandTests
  {
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<ILogger<ForgotPasswordCommand>> _loggerMock;
    private Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private Mock<IRequestClient<ISendEmailRequest>> _rcSendEmailMock;
    private Mock<IOperationResult<bool>> _operationResultSendEmailMock;

    private DbUser _dbUser;
    private IMemoryCache _memoryCache;
    private IForgotPasswordCommand _command;
    private IOptions<MemoryCacheConfig> _cacheOptions;
    private CreateUserRequest _createUserRequest;
    private DbUserCommunication _dbCommunication;
    private OperationResultResponse<bool> _expectedOperationResultResponse;

    #region Broker setup

    private void RcGetTemplateTagSetUp()
    {
      var templateTags = new Dictionary<string, string>();
      templateTags.Add("userFirstName", "");
      templateTags.Add("userId", "");
      templateTags.Add("userPassword", "");
      templateTags.Add("userEmail", "");
    }

    private void RcSendEmailSetUp()
    {
      _operationResultSendEmailMock = new Mock<IOperationResult<bool>>();
      _operationResultSendEmailMock.Setup(x => x.Body).Returns(true);
      _operationResultSendEmailMock.Setup(x => x.IsSuccess).Returns(true);
      _operationResultSendEmailMock.Setup(x => x.Errors).Returns(new List<string>());

      var responseBrokerSendEmailMock = new Mock<Response<IOperationResult<bool>>>();

      responseBrokerSendEmailMock
         .SetupGet(x => x.Message)
         .Returns(_operationResultSendEmailMock.Object);

      _rcSendEmailMock.Setup(
          x => x.GetResponse<IOperationResult<bool>>(
              It.IsAny<object>(), default, It.IsAny<RequestTimeout>()))
          .Returns(Task.FromResult(responseBrokerSendEmailMock.Object));
    }

    #endregion

    #region Setup
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _cacheOptions = Options.Create(new MemoryCacheConfig
      {
        CacheLiveInMinutes = 5
      });
      _memoryCache = new MemoryCache(new MemoryCacheOptions());
      _userRepositoryMock = new Mock<IUserRepository>();
      _loggerMock = new Mock<ILogger<ForgotPasswordCommand>>();
      _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

      _rcSendEmailMock = new Mock<IRequestClient<ISendEmailRequest>>();

      var userId = Guid.NewGuid();

      _createUserRequest = new CreateUserRequest
      {
        FirstName = "Ivan",
        LastName = "Ivanov",
        MiddleName = "Ivanovich",
        PositionId = Guid.NewGuid(),
        IsAdmin = false
      };

      _dbCommunication = new DbUserCommunication
      {
        Id = Guid.NewGuid(),
        UserId = userId,
        Type = (int)CommunicationType.Email,
        Value = "IvanoIvan@gmail.com"
      };

      _dbUser = new DbUser
      {
        Id = userId,
        FirstName = _createUserRequest.FirstName,
        LastName = _createUserRequest.LastName,
        MiddleName = _createUserRequest.MiddleName,
        IsAdmin = (bool)_createUserRequest.IsAdmin,
        IsActive = true,
        Communications = new List<DbUserCommunication>
                {
                    _dbCommunication
                }
      };

      IDictionary<object, object> httpContextItems = new Dictionary<object, object>();
      httpContextItems.Add("UserId", userId);

      _httpContextAccessorMock
          .Setup(x => x.HttpContext.Items)
          .Returns(httpContextItems);

      /*_command = new ForgotPasswordCommand(
          _loggerMock.Object,
          _rcSendEmailMock.Object,
          _cacheOptions,
          _httpContextAccessorMock.Object,
          _validatorMock.Object,
          _userRepositoryMock.Object,
          _memoryCache);*/
    }

    [SetUp]
    public void SetUp()
    {
      _expectedOperationResultResponse = new OperationResultResponse<bool>()
      {
        Body = true,
        Errors = new List<string>()
      };

      _userRepositoryMock.Reset();

      RcSendEmailSetUp();
      RcGetTemplateTagSetUp();
    }

    #endregion

    /*[Test]
    public void ShoulRequestIsPartialSuccessWhenEmailWasNotSended()
    {
        _expectedOperationResultResponse.Status = OperationResultStatusType.PartialSuccess;

        var messageError = new List<string>();
        messageError.Add($"Can not send email to '{_dbCommunication.Value}'. Please try again latter.");

        _expectedOperationResultResponse.Errors = messageError;
        _expectedOperationResultResponse.Body = false;
        _expectedOperationResultResponse.Status = OperationResultStatusType.Failed;

        _operationResultSendEmailMock
            .Setup(x => x.IsSuccess)
            .Returns(false);
        _operationResultSendEmailMock
            .Setup(x => x.Errors)
            .Returns(messageError);

        _validatorMock
            .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
            .Returns(true);

        _userRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<GetUserFilter>()))
            .Returns(_dbUser);

        SerializerAssert.AreEqual(_expectedOperationResultResponse, _command.ExecuteAsync(_dbCommunication.Value));
    }*/

    /*[Test]
    public void ShoulCreateUserSuccessful()
    {
        _validatorMock
            .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
            .Returns(true);

        _userRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<GetUserFilter>()))
            .Returns(_dbUser);

        SerializerAssert.AreEqual(_expectedOperationResultResponse, _command.ExecuteAsync(_dbCommunication.Value));
    }*/
  }
}
