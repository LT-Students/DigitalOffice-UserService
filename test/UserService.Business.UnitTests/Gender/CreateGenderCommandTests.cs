using FluentValidation.Results;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Business.Commands.User;
using LT.DigitalOffice.UserService.Business.Commands.User.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Gender;
using LT.DigitalOffice.UserService.Validation.Gender.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.UnitTests.Gender
{
  internal class CreateGenderCommandTests
  {
    private AutoMocker _mocker;
    private ICreateGenderCommand _command;
    private CreateGenderRequest _request;
    private DbGender _dbGender;
    private OperationResultResponse<Guid?> _badResponse;
    private OperationResultResponse<Guid?> _goodResponse;

    private void Verifiable(
      Times validatorCalls,
      Times responseCreatorCalls,
      Times mapperCalls,
      Times repositoryCalls)
    {
      _mocker.Verify<ICreateGenderRequestValidator>(
        x => x.ValidateAsync(It.IsAny<CreateGenderRequest>(), default),
        validatorCalls);

      _mocker.Verify<IResponseCreator>(
        x => x.CreateFailureResponse<Guid?>(It.IsAny<HttpStatusCode>(), It.IsAny<List<string>>()),
        responseCreatorCalls);

      _mocker.Verify<IDbGenderMapper>(
        x => x.Map(It.IsAny<CreateGenderRequest>()),
        mapperCalls);

      _mocker.Verify<IGenderRepository>(
        x => x.CreateAsync(It.IsAny<DbGender>()),
        repositoryCalls);

      _mocker.Resolvers.Clear();
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _dbGender = new()
      {
        Id = Guid.NewGuid(),
        Name = "Name",
        CreatedBy = Guid.NewGuid(),
        CreatedAtUtc = DateTime.UtcNow
      };

      _badResponse = new()
      {
        Body = null,
        Errors = new List<string>() { "Error" }
      };

      _goodResponse = new()
      {
        Body = _dbGender.Id,
        Errors = new List<string>()
      };

      _request = new()
      {
        Name = _dbGender.Name
      };
    }

    [SetUp]
    public void SetUp()
    {
      _mocker = new AutoMocker();

      _mocker
        .Setup<ICreateGenderRequestValidator, Task<ValidationResult>>(x => x.ValidateAsync(It.IsAny<CreateGenderRequest>(), default))
        .Returns(Task.FromResult(new ValidationResult() { }));

      _mocker
        .Setup<IResponseCreator, OperationResultResponse<Guid?>>(x =>
          x.CreateFailureResponse<Guid?>(It.IsAny<HttpStatusCode>(), It.IsAny<List<string>>()))
        .Returns(_badResponse);

      _mocker
        .Setup<IDbGenderMapper, DbGender>(x => x.Map(It.IsAny<CreateGenderRequest>()))
        .Returns(_dbGender);

      _mocker
        .Setup<IGenderRepository, Task>(x => x.CreateAsync(It.IsAny<DbGender>()))
        .Returns(Task.CompletedTask);

      _mocker
        .Setup<IHttpContextAccessor, int>(x => x.HttpContext.Response.StatusCode)
        .Returns(201);

      _command = _mocker.CreateInstance<CreateGenderCommand>();
    }

    //TODO add IsAdmin check to tests + AccessFailureTest

    //[Test]
    //public void SuccesTest()
    //{
    //  SerializerAssert.AreEqual(_goodResponse, _command.ExecuteAsync(_request).Result);

    //  Verifiable(
    //    validatorCalls: Times.Once(),
    //    responseCreatorCalls: Times.Never(),
    //    mapperCalls: Times.Once(),
    //    repositoryCalls: Times.Once());
    //}

    //[Test]
    //public void ValidationFailureTest()
    //{
    //  _mocker
    //    .Setup<ICreateGenderRequestValidator, Task<ValidationResult>>(x => x.ValidateAsync(It.IsAny<CreateGenderRequest>(), default))
    //    .Returns(Task.FromResult(new ValidationResult(new List<ValidationFailure>() { new ValidationFailure("_", "Error") })));

    //  SerializerAssert.AreEqual(_badResponse, _command.ExecuteAsync(_request).Result);

    //  Verifiable(
    //    validatorCalls: Times.Once(),
    //    responseCreatorCalls: Times.Once(),
    //    mapperCalls: Times.Never(),
    //    repositoryCalls: Times.Never());
    //}
  }
}
