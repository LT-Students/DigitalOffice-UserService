using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Broker.Consumers;
using LT.DigitalOffice.UserService.Data.Interfaces;
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
    internal class OperationResult<T> : IOperationResult<T>
    {
        public bool IsSuccess { get; set; }

        public List<string> Errors { get; set; }

        public T Body { get; set; }
    }

    internal class UserInfoResponse : IGetUserInfoResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public bool IsActive { get; set; }
    }

    internal class UserPositionResponse : IUserPositionResponse
    {
        public string UserPositionName { get; set; }
    }

    internal class GetUserInfoConsumerTests
    {
        private readonly Guid userId = Guid.NewGuid();
        private ConsumerTestHarness<GetUserInfoConsumer> consumerTestHarness;

        private InMemoryTestHarness harness;
        private DbUser dbUser;

        private Mock<IUserRepository> repository;


        [SetUp]
        public void SetUp()
        {
            repository = new Mock<IUserRepository>();

            harness = new InMemoryTestHarness();
            consumerTestHarness = harness.Consumer(() =>
                new GetUserInfoConsumer(repository.Object));

            dbUser = new DbUser
            {
                Id = userId,
                FirstName = "Ivan",
                LastName = "Ivanov",
                MiddleName = "Ivnovich",
                IsActive = false
            };
        }

        [Test]
        public async Task ShouldResponseUserInfoResponse()
        {
            repository
                .Setup(x => x.GetUserInfoById(It.IsAny<Guid>()))
                .Returns(dbUser)
                .Verifiable();

            await harness.Start();

            try
            {
                var requestClient = await harness.ConnectRequestClient<IGetUserInfoRequest>();

                var response = await requestClient.GetResponse<IOperationResult<IGetUserInfoResponse>>(new
                {
                    UserId = userId
                });

                var expected = new
                {
                    IsSuccess = true,
                    Errors = null as List<string>,
                    Body = new
                    {
                        dbUser.Id,
                        dbUser.FirstName,
                        dbUser.LastName,
                        dbUser.MiddleName,
                        dbUser.IsActive
                    }
                };

                Assert.True(response.Message.IsSuccess);
                Assert.AreEqual(null, response.Message.Errors);
                SerializerAssert.AreEqual(expected, response.Message);
                Assert.That(consumerTestHarness.Consumed.Select<IGetUserInfoRequest>().Any(), Is.True);
                Assert.That(harness.Sent.Select<IOperationResult<IGetUserInfoResponse>>().Any(), Is.True);
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
                .Setup(x => x.GetUserInfoById(It.IsAny<Guid>()))
                .Throws(new Exception("User with this id not found."));

            await harness.Start();

            try
            {
                var requestClient = await harness.ConnectRequestClient<IGetUserInfoRequest>();

                var response = await requestClient.GetResponse<IOperationResult<IGetUserInfoResponse>>(new
                {
                    UserId = userId
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