﻿using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.UnitTestLibrary;
using LT.DigitalOffice.UserService.Broker.Consumers;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using MassTransit.Testing;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Exceptions;

namespace LT.DigitalOffice.UserServiceUnitTests.Broker.Consumer
{

    public class UserCredentialsResponse : IUserCredentialsResponse
    {
        public Guid UserId { get; set; }
        public string PasswordHash { get; set; }
    }

    class UserConsumerTests
    {
        private InMemoryTestHarness harness;
        private Mock<IUserCredentialsRepository> _credentialsRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private DbUser newDbUser;
        private DbUserCredentials userCredentials;
        private ConsumerTestHarness<UserLoginConsumer> consumerTestHarness;

        #region SetUp
        [SetUp]
        public void SetUp()
        {
            harness = new InMemoryTestHarness();
            _credentialsRepositoryMock = new Mock<IUserCredentialsRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();

            var userId = Guid.NewGuid();

            newDbUser = new DbUser
            {
                Id = userId,
                FirstName = "Example1",
                LastName = "Example",
                MiddleName = "Example",
                Status = "Example",
                AvatarFileId = Guid.NewGuid(),
                IsActive = true
            };

            userCredentials = new DbUserCredentials
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PasswordHash = "Example",
                Login = "Example",
                Salt = "Example_Salt"
            };

            _credentialsRepositoryMock
               .Setup(x => x.GetUserCredentialsByLogin(It.IsAny<string>()))
               .Returns(userCredentials);

            consumerTestHarness = harness.Consumer(
                () => new UserLoginConsumer(_credentialsRepositoryMock.Object, _userRepositoryMock.Object));
        }
        #endregion

        #region ResponseToBroker
        [Test]
        public async Task ShouldSendResponseToBrokerWhenUserEmailFoundInDb()
        {
            string userEmail = "Example@gmail.com";

            var userId = Guid.NewGuid();

            var expectedResponse = new UserCredentialsResponse
            {
                UserId = userId,
                PasswordHash = userCredentials.PasswordHash
            };
            _userRepositoryMock
                .Setup(x => x.GetUserByEmail(It.IsAny<string>()))
                .Returns(new DbUser { Id = userId });

            _credentialsRepositoryMock
                .Setup(x => x.GetUserCredentialsByUserId(userId))
                .Returns(new DbUserCredentials
                {
                    UserId = userId,
                    PasswordHash = userCredentials.PasswordHash
                });

            await harness.Start();

            try
            {
                var requestClient = await harness.ConnectRequestClient<IUserCredentialsRequest>();

                var response = await requestClient.GetResponse<IOperationResult<IUserCredentialsResponse>>(
                    IUserCredentialsRequest.CreateObj(userEmail));
                //    new
                //{
                //    LoginData = userEmail
                //});

                Assert.True(response.Message.IsSuccess);
                Assert.AreEqual(null, response.Message.Errors);
                SerializerAssert.AreEqual(expectedResponse, response.Message.Body);
                Assert.That(consumerTestHarness.Consumed.Select<IUserCredentialsRequest>().Any(), Is.True);
                Assert.That(harness.Sent.Select<IOperationResult<IUserCredentialsResponse>>().Any(), Is.True);
                _credentialsRepositoryMock.Verify(repository => repository.GetUserCredentialsByUserId(It.IsAny<Guid>()), Times.Once);
            }
            finally
            {
                await harness.Stop();
            }
        }
        #endregion

        #region ThrowException
        [Test]
        public async Task ShouldExceptionWhenUserEmailNotFoundInDb()
        {
            string userEmail = "Example@gmail.com";

            UserCredentialsResponse expectedResponse = null;

            await harness.Start();

            try
            {
                var requestClient = await harness.ConnectRequestClient<IUserCredentialsRequest>();

                var response = await requestClient.GetResponse<IOperationResult<IUserCredentialsResponse>>(new
                {
                    LoginData = userEmail
                });

                Assert.False(response.Message.IsSuccess);
                Assert.AreEqual(
                    "User with email: 'Example@gmail.com' was not found.",
                    response.Message.Errors[0]);
                SerializerAssert.AreEqual(expectedResponse, response.Message.Body);
                Assert.That(consumerTestHarness.Consumed.Select<IUserCredentialsRequest>().Any(), Is.True);
                Assert.That(harness.Sent.Select<IOperationResult<IUserCredentialsResponse>>().Any(), Is.True);
                _userRepositoryMock.Verify(repository => repository.GetUserByEmail(It.IsAny<string>()), Times.Once);
            }
            finally
            {
                await harness.Stop();
            }
        }
        #endregion
    }
}
