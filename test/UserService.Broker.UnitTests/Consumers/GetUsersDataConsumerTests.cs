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
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.User;
using LT.DigitalOffice.UserService.Models.Dto.Enums;

namespace LT.DigitalOffice.UserService.Broker.UnitTests.Consumers
{
    internal class GetUsersDataConsumerTests
    {
        private readonly Guid _userId = Guid.NewGuid();
        private ConsumerTestHarness<GetUsersDataConsumer> _consumerTestHarness;

        private InMemoryTestHarness _harness;
        private DbUser _dbUser;
        private UserData _userData;

        private Mock<IUserRepository> _repository;

        [SetUp]
        public void SetUp()
        {
            _repository = new Mock<IUserRepository>();

            _harness = new InMemoryTestHarness();
            _consumerTestHarness = _harness.Consumer(() =>
                new GetUsersDataConsumer(_repository.Object));

            _dbUser = new DbUser
            {
                Id = _userId,
                FirstName = "Ivan",
                LastName = "Ivanov",
                MiddleName = "Ivnovich",
                IsActive = false,
                AvatarFileId = Guid.NewGuid(),
                Rate = 0.25
            };

            _userData = new UserData(
                _dbUser.Id,
                _dbUser.AvatarFileId,
                _dbUser.FirstName,
                _dbUser.MiddleName,
                _dbUser.LastName,
                ((UserStatus)_dbUser.Status).ToString(),
                (float)_dbUser.Rate,
                _dbUser.IsActive);
        }

        [Test]
        public async Task ShouldResponseUserDataResponse()
        {
            _repository
                .Setup(x => x.Get(It.IsAny<IEnumerable<Guid>>()))
                .Returns(new List<DbUser> { _dbUser })
                .Verifiable();

            await _harness.Start();

            try
            {
                var requestClient = await _harness.ConnectRequestClient<IGetUsersDataRequest>();

                var response = await requestClient.GetResponse<IOperationResult<IGetUsersDataResponse>>(new
                {
                    UserIds = new List<Guid> { _userId }
                });

                var expected = new
                {
                    IsSuccess = true,
                    Errors = null as List<string>,
                    Body = new { UsersData = new List<UserData> { _userData } }
                };

                Assert.True(response.Message.IsSuccess);
                Assert.AreEqual(null, response.Message.Errors);
                SerializerAssert.AreEqual(expected, response.Message);
                Assert.True(_consumerTestHarness.Consumed.Select<IGetUsersDataRequest>().Any());
                Assert.True(_harness.Sent.Select<IOperationResult<IGetUsersDataResponse>>().Any());
                _repository.Verify();
            }
            finally
            {
                await _harness.Stop();
            }
        }

        [Test]
        public async Task ShouldResponseIOperationResultWithExceptionWhenRepositoryNotFoundUser()
        {
            _repository
                .Setup(x => x.Get(It.IsAny<IEnumerable<Guid>>()))
                .Throws(new Exception("User with this id not found."));

            await _harness.Start();

            try
            {
                var requestClient = await _harness.ConnectRequestClient<IGetUsersDataRequest>();

                var response = await requestClient.GetResponse<IOperationResult<IGetUsersDataResponse>>(new
                {
                    UserIds = new List<Guid> { _userId }
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
                await _harness.Stop();
            }
        }
    }
}