using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Broker.Consumers;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserServiceUnitTests.Broker.Consumer
{
    class UserLoginConsumerTests
    {
        private InMemoryTestHarness harness;
        private Mock<IUserCredentialsRepository> credentialsRepositoryMock;
        private Mock<IUserRepository> userRepositoryMock;
        private DbUserCredentials userCredentials;
        private ConsumerTestHarness<UserLoginConsumer> consumerTestHarness;
        private Mock<ILogger<UserLoginConsumer>> _loggerMock;

        #region SetUp
        //[SetUp]
        //public void SetUp()
        //{
        //    harness = new InMemoryTestHarness();
        //    credentialsRepositoryMock = new Mock<IUserCredentialsRepository>();
        //    userRepositoryMock = new Mock<IUserRepository>();

        //    var userId = Guid.NewGuid();

        //    userCredentials = new DbUserCredentials
        //    {
        //        Id = Guid.NewGuid(),
        //        UserId = userId,
        //        PasswordHash = "Example",
        //        Login = "Example",
        //        Salt = "Example_Salt"
        //    };

        //    credentialsRepositoryMock
        //       .Setup(x => x.Get(It.IsAny<string>()))
        //       .Returns(userCredentials);

        //    consumerTestHarness = harness.Consumer(
        //        () => new UserLoginConsumer(credentialsRepositoryMock.Object, userRepositoryMock.Object));
        //}
        //#endregion

        //#region ResponseToBroker
        //[Test]
        //public async Task ShouldSendResponseToBrokerWhenUserEmailFoundInDb()
        //{
        //    string userEmail = "Example@gmail.com";

        //    var userId = Guid.NewGuid();

        //    var expectedResponse = new UserCredentialsResponse
        //    {
        //        UserId = userId,
        //        PasswordHash = userCredentials.PasswordHash,
        //        Salt = userCredentials.Salt,
        //        UserLogin = userCredentials.Login
        //    };

        //    credentialsRepositoryMock
        //        .Setup(x => x.Get(userId))
        //        .Returns(new DbUserCredentials
        //        {
        //            UserId = userId,
        //            PasswordHash = userCredentials.PasswordHash,
        //            Login = userCredentials.Login,
        //            Salt = userCredentials.Salt
        //        });

        //    await harness.Start();

        //    try
        //    {
        //        var requestClient = await harness.ConnectRequestClient<IUserCredentialsRequest>();

        //        var response = await requestClient.GetResponse<IOperationResult<IUserCredentialsResponse>>(
        //            IUserCredentialsRequest.CreateObj(userEmail));

        //        Assert.True(response.Message.IsSuccess);
        //        Assert.AreEqual(null, response.Message.Errors);
        //        SerializerAssert.AreEqual(expectedResponse, response.Message.Body);
        //        Assert.That(consumerTestHarness.Consumed.Select<IUserCredentialsRequest>().Any(), Is.True);
        //        Assert.That(harness.Sent.Select<IOperationResult<IUserCredentialsResponse>>().Any(), Is.True);
        //        credentialsRepositoryMock.Verify(repository => repository.Get(It.IsAny<Guid>()), Times.Once);
        //    }
        //    finally
        //    {
        //        await harness.Stop();
        //    }
        //}

        //[Test]
        //public async Task ShouldSendResponseToBrokerWhenUserLoginFoundInDb()
        //{
        //    string userLogin = "User_login_example";

        //    var userId = Guid.NewGuid();

        //    var expectedResponse = new UserCredentialsResponse
        //    {
        //        UserId = userId,
        //        PasswordHash = userCredentials.PasswordHash,
        //        Salt = userCredentials.Salt,
        //        UserLogin = userCredentials.Login
        //    };

        //    credentialsRepositoryMock
        //        .Setup(x => x.Get(userLogin))
        //        .Returns(new DbUserCredentials
        //        {
        //            UserId = userId,
        //            PasswordHash = userCredentials.PasswordHash,
        //            Login = userCredentials.Login,
        //            Salt = userCredentials.Salt
        //        });

        //    await harness.Start();

        //    try
        //    {
        //        var requestClient = await harness.ConnectRequestClient<IUserCredentialsRequest>();

        //        var response = await requestClient.GetResponse<IOperationResult<IUserCredentialsResponse>>(
        //            IUserCredentialsRequest.CreateObj(userLogin));

        //        Assert.True(response.Message.IsSuccess);
        //        Assert.AreEqual(null, response.Message.Errors);
        //        SerializerAssert.AreEqual(expectedResponse, response.Message.Body);
        //        Assert.That(consumerTestHarness.Consumed.Select<IUserCredentialsRequest>().Any(), Is.True);
        //        Assert.That(harness.Sent.Select<IOperationResult<IUserCredentialsResponse>>().Any(), Is.True);
        //        credentialsRepositoryMock.Verify(repository => repository.Get(It.IsAny<string>()), Times.Once);
        //    }
        //    finally
        //    {
        //        await harness.Stop();
        //    }
        //}
        //#endregion

        //#region ThrowException
        //[Test]
        //public async Task ShouldExceptionWhenUserEmailNotFoundInDb()
        //{
        //    string userEmail = "Example@gmail.com";

        //    UserCredentialsResponse expectedResponse = null;

        //    await harness.Start();

        //    try
        //    {
        //        var requestClient = await harness.ConnectRequestClient<IUserCredentialsRequest>();

        //        var response = await requestClient.GetResponse<IOperationResult<IUserCredentialsResponse>>(new
        //        {
        //            LoginData = userEmail
        //        });

        //        Assert.False(response.Message.IsSuccess);
        //        Assert.AreEqual(
        //            "User with email: 'Example@gmail.com' was not found.",
        //            response.Message.Errors[0]);
        //        SerializerAssert.AreEqual(expectedResponse, response.Message.Body);
        //        Assert.That(consumerTestHarness.Consumed.Select<IUserCredentialsRequest>().Any(), Is.True);
        //        Assert.That(harness.Sent.Select<IOperationResult<IUserCredentialsResponse>>().Any(), Is.True);
        //    }
        //    finally
        //    {
        //        await harness.Stop();
        //    }
        //}
        #endregion
    }
}
