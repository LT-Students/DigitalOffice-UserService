using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Broker.Consumers;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.Broker.Models;
using LT.DigitalOffice.UserService.Models.Db;
using MassTransit;
using MassTransit.Testing;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.UnitTests.Consumers
{
    internal class GetUsersDataConsumerTests
    {
        private readonly Guid userId = Guid.NewGuid();
        private ConsumerTestHarness<GetUsersDataConsumer> consumerTestHarness;

        private InMemoryTestHarness harness;
        private DbUser dbUser;
        private UserData userData;

        private Mock<IUserRepository> repository;


        [SetUp]
        public void SetUp()
        {
            repository = new Mock<IUserRepository>();

            harness = new InMemoryTestHarness();
            consumerTestHarness = harness.Consumer(() =>
                new GetUsersDataConsumer(repository.Object));

            dbUser = new DbUser
            {
                Id = userId,
                FirstName = "Ivan",
                LastName = "Ivanov",
                MiddleName = "Ivnovich",
                IsActive = false
            };

            userData = UserData.Create(dbUser.Id, dbUser.FirstName, dbUser.MiddleName, dbUser.LastName, dbUser.IsActive);
        }

        [Test]
        public async Task ShouldResponseUserDataResponse()
        {
            repository
                .Setup(x => x.Get(It.IsAny<IEnumerable<Guid>>()))
                .Returns(new List<DbUser> { dbUser })
                .Verifiable();

            await harness.Start();

            try
            {
                var requestClient = await harness.ConnectRequestClient<IGetUsersDataRequest>();

                var response = await requestClient.GetResponse<IOperationResult<IGetUsersDataResponse>>(new
                {
                    UserIds = new List<Guid> { userId }
                });

                var expected = new
                {
                    IsSuccess = true,
                    Errors = null as List<string>,
                    Body = new { UsersData = new List<UserData> { userData } }
                };

                Assert.True(response.Message.IsSuccess);
                Assert.AreEqual(null, response.Message.Errors);
                SerializerAssert.AreEqual(expected, response.Message);
                Assert.True(consumerTestHarness.Consumed.Select<IGetUsersDataRequest>().Any());
                Assert.True(harness.Sent.Select<IOperationResult<IGetUsersDataResponse>>().Any());
                repository.Verify();
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Test]
        public async Task ShouldResponseIOperationResultWithExceptionWhenRepositoryNotFoundUser()
        {
            repository
                .Setup(x => x.Get(It.IsAny<IEnumerable<Guid>>()))
                .Throws(new Exception("User with this id not found."));

            await harness.Start();

            try
            {
                var requestClient = await harness.ConnectRequestClient<IGetUsersDataRequest>();

                var response = await requestClient.GetResponse<IOperationResult<IGetUsersDataResponse>>(new
                {
                    UserIds = new List<Guid> { userId }
                });

                var expected = new
                {
                    IsSuccess = false,
                    Errors = new List<string> { "User with this id not found." },
                    Body = null as object
                };

                SerializerAssert.AreEqual(expected, response.Message);
            }
            finally
            {
                await harness.Stop();
            }
        }
    }
}