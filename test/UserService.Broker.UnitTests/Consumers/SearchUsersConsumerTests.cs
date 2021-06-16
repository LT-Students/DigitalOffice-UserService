using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.Search;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Broker.Consumers;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using MassTransit.Testing;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.UnitTests.Consumers
{
    public class SearchUsersConsumerTests
    {
        private ConsumerTestHarness<SearchUsersConsumer> _consumerTestHarness;

        private InMemoryTestHarness _harness;
        private List<DbUser> _dbUsers;
        private List<SearchInfo> _result;

        private const string ExistName = "Na";

        private Mock<IUserRepository> _repository;

        [SetUp]
        public void SetUp()
        {
            _harness = new InMemoryTestHarness();
            _consumerTestHarness = _harness.Consumer(() =>
                new SearchUsersConsumer(_repository.Object));

            _dbUsers = new()
            {
                new DbUser
                {
                    Id = Guid.NewGuid(),
                    FirstName = "SName1",
                    LastName = "Lastname"
                },
                new DbUser
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Nme",
                    LastName = "LastName2"

                },
                new DbUser
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Name3",
                    LastName = "LastName3"
                }
            };

            _result = new()
            {
                new SearchInfo(_dbUsers[0].Id, _dbUsers[0].LastName + " " + _dbUsers[0].FirstName),
                new SearchInfo(_dbUsers[1].Id, _dbUsers[1].LastName + " " + _dbUsers[1].FirstName),
                new SearchInfo(_dbUsers[2].Id, _dbUsers[2].LastName + " " + _dbUsers[2].FirstName)
            };

            _repository = new();
            _repository
                .Setup(r => r.Search(ExistName))
                .Returns(_dbUsers);
        }

        [Test]
        public async Task ShouldConsumeSuccessful()
        {
            await _harness.Start();

            try
            {
                var requestClient = await _harness.ConnectRequestClient<ISearchUsersRequest>();

                var response = await requestClient.GetResponse<IOperationResult<ISearchResponse>>(
                    ISearchUsersRequest.CreateObj(ExistName), default, default);

                var expectedResult = new
                {
                    IsSuccess = true,
                    Errors = null as List<string>,
                    Body = new
                    {
                        Entities = _result
                    }
                };

                SerializerAssert.AreEqual(expectedResult, response.Message);
                Assert.True(_consumerTestHarness.Consumed.Select<ISearchUsersRequest>().Any());
                Assert.True(_harness.Sent.Select<IOperationResult<ISearchResponse>>().Any());
                _repository.Verify(x => x.Search(ExistName), Times.Once);
            }
            finally
            {
                await _harness.Stop();
            }
        }

        [Test]
        public async Task ShouldThrowExceptionWhenRepositoryThrow()
        {
            _repository
                .Setup(r => r.Search(ExistName))
                .Throws(new Exception());

            await _harness.Start();

            try
            {
                var requestClient = await _harness.ConnectRequestClient<ISearchUsersRequest>();

                var response = await requestClient.GetResponse<IOperationResult<ISearchResponse>>(
                    ISearchUsersRequest.CreateObj(ExistName), default, default);

                var expectedResult = new
                {
                    IsSuccess = false,
                    Errors = new List<string> { "some error" },
                };

                Assert.IsFalse(response.Message.IsSuccess);
                Assert.IsNotEmpty(response.Message.Errors);
                Assert.True(_consumerTestHarness.Consumed.Select<ISearchUsersRequest>().Any());
                Assert.True(_harness.Sent.Select<IOperationResult<ISearchResponse>>().Any());
                _repository.Verify(x => x.Search(ExistName), Times.Once);
            }
            finally
            {
                await _harness.Stop();
            }
        }
    }
}
