using LT.DigitalOffice.Broker.Requests;
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
using System.Threading.Tasks;

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
        private Mock<IUserCredentialsRepository> repository;
        private DbUser newDbUser;
        private DbUserCredentials userCredentials;
        private ConsumerTestHarness<UserLoginConsumer> consumerTestHarness;

        #region SetUp
        [SetUp]
        public void SetUp()
        {
            harness = new InMemoryTestHarness();
            repository = new Mock<IUserCredentialsRepository>();

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
                Email = "Example@gmail.com",
                Salt = "Example_Salt"
            };

            repository
               .Setup(x => x.GetUserCredentialsByEmail(It.IsAny<string>()))
               .Returns(userCredentials);

            consumerTestHarness = harness.Consumer(() => new UserLoginConsumer(repository.Object));
        }
        #endregion

        #region ResponseToBroker
        [Test]
        public async Task ShouldSendResponseToBrokerWhenUserEmailFoundInDb()
        {
            string userEmail = "Example@gmail.com";

            var expectedResponse = new UserCredentialsResponse
            {
                UserId = newDbUser.Id,
                PasswordHash = userCredentials.PasswordHash
            };

            await harness.Start();

            try
            {
                var requestClient = await harness.ConnectRequestClient<IUserCredentialsRequest>();

                var response = await requestClient.GetResponse<IOperationResult<IUserCredentialsResponse>>( new
                {
                    Email = userEmail
                });

                Assert.That(response.Message.IsSuccess, Is.True);
                Assert.AreEqual(null, response.Message.Errors);
                SerializerAssert.AreEqual(expectedResponse, response.Message.Body);
                Assert.That(consumerTestHarness.Consumed.Select<IUserCredentialsRequest>().Any(), Is.True);
                Assert.That(harness.Sent.Select<IOperationResult<IUserCredentialsResponse>>().Any(), Is.True);
                repository.Verify(repository => repository.GetUserCredentialsByEmail(It.IsAny<string>()), Times.Once);
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

            repository
                .Setup(x => x.GetUserCredentialsByEmail(It.IsAny<string>()))
                .Throws(new ArgumentNullException());

            await harness.Start();

            try
            {
                var requestClient = await harness.ConnectRequestClient<IUserCredentialsRequest>();

                var response = await requestClient.GetResponse<IOperationResult<IUserCredentialsResponse>>(new
                {
                    Email = userEmail
                });

                Assert.That(response.Message.IsSuccess, Is.False);
                Assert.AreEqual(new ArgumentNullException().Message, String.Join(", ", response.Message.Errors));
                SerializerAssert.AreEqual(expectedResponse, response.Message.Body);
                Assert.That(consumerTestHarness.Consumed.Select<IUserCredentialsRequest>().Any(), Is.True);
                Assert.That(harness.Sent.Select<IOperationResult<IUserCredentialsResponse>>().Any(), Is.True);
                repository.Verify(repository => repository.GetUserCredentialsByEmail(It.IsAny<string>()), Times.Once);
            }
            finally
            {
                await harness.Stop();
            }
        }
        #endregion
    }
}
