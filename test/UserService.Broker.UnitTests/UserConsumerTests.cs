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

namespace LT.DigitalOffice.UserService.Broker.UnitTests
{
    public class UserCredentialsResponse : IUserCredentialsResponse
    {
        public Guid UserId { get; set; }
        public string PasswordHash { get; set; }
    }

    class UserConsumerTests
    {
        private InMemoryTestHarness harness;
        private Mock<IUserRepository> repositoryMock;
        private DbUser newDbUser;
        private ConsumerTestHarness<LoginUserConsumer> consumerTestHarness;

        #region SetUp
        [SetUp]
        public void SetUp()
        {
            harness = new InMemoryTestHarness();
            repositoryMock = new Mock<IUserRepository>();

            newDbUser = new DbUser
            {
                Id = Guid.NewGuid(),
                FirstName = "Example1",
                LastName = "Example",
                MiddleName = "Example",
                Email = "Example@gmail.com",
                Status = "Example",
                PasswordHash = "Example",
                AvatarFileId = Guid.NewGuid(),
                IsActive = true
            };

            repositoryMock
                .Setup(x => x.GetUserByEmail(It.IsAny<string>()))
                .Returns(newDbUser);

            consumerTestHarness = harness.Consumer(() => new LoginUserConsumer(repositoryMock.Object));
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
                PasswordHash = newDbUser.PasswordHash
            };

            await harness.Start();

            try
            {
                var requestClient = await harness.ConnectRequestClient<IUserCredentialsRequest>();

                var response = await requestClient.GetResponse<IOperationResult<IUserCredentialsResponse>>(new
                {
                    Email = userEmail
                });

                Assert.That(response.Message.IsSuccess, Is.True);
                Assert.AreEqual(null, response.Message.Errors);
                SerializerAssert.AreEqual(expectedResponse, response.Message.Body);
                Assert.That(consumerTestHarness.Consumed.Select<IUserCredentialsRequest>().Any(), Is.True);
                Assert.That(harness.Sent.Select<IOperationResult<IUserCredentialsResponse>>().Any(), Is.True);
                repositoryMock.Verify(repository => repository.GetUserByEmail(It.IsAny<string>()), Times.Once);
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

            repositoryMock
                .Setup(x => x.GetUserByEmail(It.IsAny<string>()))
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
                repositoryMock.Verify(repository => repository.GetUserByEmail(It.IsAny<string>()), Times.Once);
            }
            finally
            {
                await harness.Stop();
            }
        }
        #endregion
    }
}
