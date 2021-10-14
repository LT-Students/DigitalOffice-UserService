using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data.UnitTests
{
  class UserCredentialsRepositoryTests
  {
    private IDataProvider _provider;
    private IUserCredentialsRepository _repository;
    private Mock<ILogger<UserCredentialsRepository>> _loggerMock;
    private AutoMocker _mocker;

    private DbContextOptions<UserServiceDbContext> _dbContext;

    private DbUserCredentials _dbUserCredentials;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _dbUserCredentials = new DbUserCredentials()
      {
        Id = Guid.NewGuid(),
        UserId = Guid.NewGuid(),
        Login = "login",
        PasswordHash = "passwordHash",
        Salt = "salt",
        User = new DbUser()
        {
          Communications = new List<DbUserCommunication>()
            {
                new DbUserCommunication()
                {
                    Type = (int)CommunicationType.Email,
                    Value = "email"
                },
                new DbUserCommunication()
                {
                    Type = (int)CommunicationType.Phone,
                    Value = "123"
                }
            }
        }
      };

      _dbContext = new DbContextOptionsBuilder<UserServiceDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
            .Options;

      _mocker = new AutoMocker();

      _mocker
          .Setup<IHttpContextAccessor, IPAddress>(x => x.HttpContext.Connection.RemoteIpAddress)
          .Returns(new IPAddress(new byte[] { 129, 144, 50, 59 }));

      _loggerMock = new Mock<ILogger<UserCredentialsRepository>>();
    }

    [SetUp]
    public async Task SetUp()
    {
      _provider = new UserServiceDbContext(_dbContext);
      _repository = new UserCredentialsRepository(
          _loggerMock.Object,
          _mocker.GetMock<IHttpContextAccessor>().Object,
          _provider);

      _provider.UserCredentials.Add(_dbUserCredentials);
      await _provider.SaveAsync();
    }

    [TearDown]
    public void CleanDb()
    {
      if (_provider.IsInMemory())
      {
        _provider.EnsureDeleted();
      }
    }

    #region Get
    /*[Test]
    public void ShouldGetUserCredentialsSuccesfulByUserId()
    {
        var filter = new GetCredentialsFilter() { UserId = _dbUserCredentials.UserId };

        Assert.AreEqual(_dbUserCredentials, _repository.Get(filter));
    }

    [Test]
    public void ShouldGetUserCredentialsSuccesfulByLogin()
    {
        var filter = new GetCredentialsFilter() { Login = _dbUserCredentials.Login };

        Assert.AreEqual(_dbUserCredentials, _repository.Get(filter));
    }

    [Test]
    public void ShouldGetUserCredentialsSuccesfulByEmail()
    {
        var filter = new GetCredentialsFilter() { Email = "email" };

        Assert.AreEqual(_dbUserCredentials, _repository.Get(filter));
    }

    [Test]
    public void ShouldGetUserCredentialsSuccesfulByPhone()
    {
        var filter = new GetCredentialsFilter() { Phone = "123" };

        Assert.AreEqual(_dbUserCredentials, _repository.Get(filter));
    }*/

    [Test]
    public void ShouldThrowExceptionWhenfiltrIsEmpty()
    {
      var filter = new GetCredentialsFilter(); ;

      Assert.Throws<BadRequestException>(() => _repository.Get(filter));
    }
    #endregion

    //#region Edit
    //[Test]
    //public void ShouldThrowExceptionWhenDbUserCredentialsIsNull()
    //{
    //  DbUserCredentials userCeredentials = null;

    //  Assert.Throws<ArgumentNullException>(() => _repository.Edit(userCeredentials));
    //}

    //[Test]
    //public void ShouldThrowExceptionWhenUserIdIsNotFound()
    //{
    //  DbUserCredentials userCeredentials = new() { UserId = Guid.NewGuid() };

    //  Assert.Throws<NotFoundException>(() => _repository.Edit(userCeredentials));
    //}
    //#endregion

  }
}
