using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.UnitTestLibrary;
using LT.DigitalOffice.UserService.Broker.Consumers;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using MassTransit;
using MassTransit.Testing;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.UnitTests.Consumers
{
    public class UserExistsAndActiveResponse : IUserExistsAndActiveResponse
    {
        public bool IsUserExists { get; set; }

        public bool? IsActive { get; set; }
    }

    class IsUserExistsAndActiveConsumerTests
    {
        //private InMemoryTestHarness harness;
        //private Mock<IUserCredentialsRepository> credentialsRepositoryMock;
        //private Mock<IUserRepository> userRepositoryMock;
        //private DbUserCredentials userCredentials;
        //private ConsumerTestHarness<UserLoginConsumer> consumerTestHarness;

        //#region SetUp
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
        //       .Setup(x => x.GetUserCredentialsByLogin(It.IsAny<string>()))
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

        //    var expectedResponse = new UserExistsAndActiveResponse
        //    {
        //        UserId = userId,
        //        PasswordHash = userCredentials.PasswordHash,
        //        Salt = userCredentials.Salt,
        //        UserLogin = userCredentials.Login
        //    };

        //    userRepositoryMock
        //        .Setup(x => x.GetUserByEmail(It.IsAny<string>()))
        //        .Returns(new DbUser { Id = userId });

        //    credentialsRepositoryMock
        //        .Setup(x => x.GetUserCredentialsByUserId(userId))
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
        //        credentialsRepositoryMock.Verify(repository => repository.GetUserCredentialsByUserId(It.IsAny<Guid>()), Times.Once);
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

        //    var expectedResponse = new UserExistsAndActiveResponse
        //    {
        //        UserId = userId,
        //        PasswordHash = userCredentials.PasswordHash,
        //        Salt = userCredentials.Salt,
        //        UserLogin = userCredentials.Login
        //    };

        //    credentialsRepositoryMock
        //        .Setup(x => x.GetUserCredentialsByLogin(userLogin))
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
        //        credentialsRepositoryMock.Verify(repository => repository.GetUserCredentialsByLogin(It.IsAny<string>()), Times.Once);
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

        //    UserExistsAndActiveResponse expectedResponse = null;

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
        //        userRepositoryMock.Verify(repository => repository.GetUserByEmail(It.IsAny<string>()), Times.Once);
        //    }
        //    finally
        //    {
        //        await harness.Stop();
        //    }
        //}
        //#endregion
    }
}