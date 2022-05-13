using LT.DigitalOffice.UserService.Broker.Consumers;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using MassTransit;
using MassTransit.Testing;
using Moq;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.UserService.Broker.UnitTests.Consumers
{
    internal class GetUserDataConsumerTests
    {
        /*private readonly Guid userId = Guid.NewGuid();
        private ConsumerTestHarness<GetUserDataConsumer> consumerTestHarness;

        private InMemoryTestHarness harness;
        private DbUser dbUser;

        private Mock<IUserRepository> repository;


        [SetUp]
        public void SetUp()
        {
            repository = new Mock<IUserRepository>();

            harness = new InMemoryTestHarness();
            consumerTestHarness = harness.Consumer(() =>
                new GetUserDataConsumer(repository.Object));

            dbUser = new DbUser
            {
                Id = userId,
                FirstName = "Ivan",
                LastName = "Ivanov",
                MiddleName = "Ivnovich",
                IsActive = false
            };
        }*/

        // TODO fix

        //[Test]
        //public async Task ShouldResponseUserDataResponse()
        //{
        //    repository
        //        .Setup(x => x.Get(It.IsAny<Guid>()))
        //        .Returns(dbUser)
        //        .Verifiable();

        //    await harness.Start();

        //    try
        //    {
        //        var requestClient = await harness.ConnectRequestClient<IGetUserDataRequest>();

        //        var response = await requestClient.GetResponse<IOperationResult<IGetUserDataResponse>>(new
        //        {
        //            UserId = userId
        //        });

        //        var expected = new
        //        {
        //            IsSuccess = true,
        //            Errors = null as List<string>,
        //            Body = new
        //            {
        //                dbUser.Id,
        //                dbUser.FirstName,
        //                dbUser.LastName,
        //                dbUser.MiddleName,
        //                dbUser.IsActive
        //            }
        //        };

        //        Assert.True(response.Message.IsSuccess);
        //        Assert.AreEqual(null, response.Message.Errors);
        //        SerializerAssert.AreEqual(expected, response.Message);
        //        Assert.True(consumerTestHarness.Consumed.Select<IGetUserDataRequest>().Any());
        //        Assert.True(harness.Sent.Select<IOperationResult<IGetUserDataResponse>>().Any());
        //        repository.Verify();
        //    }
        //    finally
        //    {
        //        await harness.Stop();
        //    }
        //}

        //[Test]
        //public async Task ShouldResponseIOperationResultWithExceptionWhenRepositoryNotFoundUser()
        //{
        //    repository
        //        .Setup(x => x.Get(It.IsAny<Guid>()))
        //        .Throws(new Exception("User with this id not found."));

        //    await harness.Start();

        //    try
        //    {
        //        var requestClient = await harness.ConnectRequestClient<IGetUserDataRequest>();

        //        var response = await requestClient.GetResponse<IOperationResult<IGetUserDataResponse>>(new
        //        {
        //            UserId = userId
        //        });

        //        var expected = new
        //        {
        //            IsSuccess = false,
        //            Errors = new List<string> { "User with this id not found." },
        //            Body = null as object
        //        };

        //        SerializerAssert.AreEqual(expected, response.Message);
        //    }
        //    finally
        //    {
        //        await harness.Stop();
        //    }
        //}
    }
}