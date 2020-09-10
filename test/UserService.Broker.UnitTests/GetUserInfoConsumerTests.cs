using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.UnitTestLibrary;
using LT.DigitalOffice.UserService.Broker.Consumers;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using MassTransit;
using MassTransit.Testing;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.UnitTests
{
    internal class OperationResult<T> : IOperationResult<T>
    {
        public bool IsSuccess { get; set; }

        public List<string> Errors { get; set; }

        public T Body { get; set; }
    }

    internal class UserInfoResponse : IUserInfoResponse
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public IUserPositionResponse UserPosition { get; set; }
    }

    internal class UserPositionResponse : IUserPositionResponse
    {
        public string UserPositionName { get; set; }
    }

    internal class GetUserInfoConsumerTests
    {
        private readonly string exceptionFromCompanyService = "Exception From CompanyService";
        private readonly string firstName = "Ivan";
        private readonly string lastName = "Ivananovich";
        private readonly string midName = "Pupkin";
        private readonly Guid userId = Guid.NewGuid();
        private readonly string userPositionName = "software engineer";
        private ConsumerTestHarness<GetUserInfoConsumer> consumerTestHarness;

        private InMemoryTestHarness harness;

        private Mock<IUserRepository> repository;
        private Mock<IRequestClient<IGetUserPositionRequest>> requestBrokerMock;
        private Mock<Response<IOperationResult<IUserPositionResponse>>> responseBrokerMock;


        [SetUp]
        public void SetUp()
        {
            repository = new Mock<IUserRepository>();

            responseBrokerMock = new Mock<Response<IOperationResult<IUserPositionResponse>>>();
            requestBrokerMock = new Mock<IRequestClient<IGetUserPositionRequest>>();

            harness = new InMemoryTestHarness();
            consumerTestHarness = harness.Consumer(() =>
                new GetUserInfoConsumer(repository.Object, requestBrokerMock.Object));
        }

        /*[Test]
        public async Task ShouldResponseUserInfoResponse()
        {
            repository
                .Setup(x => x.GetUserInfoById(It.IsAny<Guid>()))
                .Returns(new DbUser
                {
                    FirstName = firstName,
                    LastName = lastName,
                    MiddleName = midName
                });


            responseBrokerMock
                .Setup(x => x.Message)
                .Returns(new OperationResult<IUserPositionResponse>
                {
                    Body = new UserPositionResponse
                    {
                        UserPositionName = userPositionName
                    },
                    IsSuccess = true,
                    Errors = null
                });

            requestBrokerMock.Setup(
                    x => x.GetResponse<IOperationResult<IUserPositionResponse>>(
                        It.IsAny<object>(), default, default))
                .Returns(Task.FromResult(responseBrokerMock.Object));

            await harness.Start();

            try
            {
                var requestClient = await harness.ConnectRequestClient<IGetUserInfoRequest>();

                var response = await requestClient.GetResponse<IOperationResult<IUserInfoResponse>>(new
                {
                    UserId = userId
                });

                var expected = new
                {
                    IsSuccess = true,
                    Errors = null as List<string>,
                    Body = new
                    {
                        UserId = userId,
                        FirstName = firstName,
                        LastName = lastName,
                        MiddleName = midName,
                        UserPosition = new
                        {
                            UserPositionName = userPositionName
                        }
                    }
                };

                SerializerAssert.AreEqual(expected, response.Message);
            }
            finally
            {
                await harness.Stop();
            }
        }*/

        [Test]
        public async Task ShouldResponseIOperationResultWithExceptionWhenIRequestClientResponseError()
        {
            responseBrokerMock
                .Setup(x => x.Message)
                .Returns(new OperationResult<IUserPositionResponse>
                {
                    IsSuccess = false,
                    Errors = new List<string> { exceptionFromCompanyService }
                });

            requestBrokerMock.Setup(
                    x => x.GetResponse<IOperationResult<IUserPositionResponse>>(
                        It.IsAny<object>(), default, default))
                .Returns(Task.FromResult(responseBrokerMock.Object));

            await harness.Start();

            try
            {
                var requestClient = await harness.ConnectRequestClient<IGetUserInfoRequest>();

                var response = await requestClient.GetResponse<IOperationResult<IUserInfoResponse>>(new
                {
                    UserId = userId
                });

                var expected = new
                {
                    IsSuccess = false,
                    Errors = new List<string> { exceptionFromCompanyService },
                    Body = null as object
                };

                SerializerAssert.AreEqual(expected, response.Message);
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

            responseBrokerMock
                .Setup(x => x.Message)
                .Returns(new OperationResult<IUserPositionResponse>
                {
                    Body = new UserPositionResponse
                    {
                        UserPositionName = userPositionName
                    },
                    IsSuccess = true,
                    Errors = null
                });

            requestBrokerMock.Setup(
                    x => x.GetResponse<IOperationResult<IUserPositionResponse>>(
                        It.IsAny<object>(), default, default))
                .Returns(Task.FromResult(responseBrokerMock.Object));

            await harness.Start();

            try
            {
                var requestClient = await harness.ConnectRequestClient<IGetUserInfoRequest>();

                var response = await requestClient.GetResponse<IOperationResult<IUserInfoResponse>>(new
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

        /*[Test]
        public async Task ShouldResponseIOperationResultWithExceptionWhenFirstArgumentOfMapperIsNull()
        {
            repository
                .Setup(x => x.GetUserInfoById(It.IsAny<Guid>()))
                .Returns(new DbUser
                {
                    FirstName = firstName,
                    LastName = lastName,
                    MiddleName = midName
                });

            mapper
                .Setup(x => x.Map(It.IsAny<DbUser>(), It.IsAny<IUserPositionResponse>()))
                .Throws(new ArgumentNullException(nameof(DbUser)));

            responseBrokerMock
                .Setup(x => x.Message)
                .Returns(new OperationResult<IUserPositionResponse>
                {
                    Body = new UserPositionResponse
                    {
                        UserPositionName = userPositionName
                    },
                    IsSuccess = true,
                    Errors = null
                });

            requestBrokerMock.Setup(
                    x => x.GetResponse<IOperationResult<IUserPositionResponse>>(
                        It.IsAny<object>(), default, default))
                .Returns(Task.FromResult(responseBrokerMock.Object));

            await harness.Start();

            try
            {
                var requestClient = await harness.ConnectRequestClient<IGetUserInfoRequest>();

                var response = await requestClient.GetResponse<IOperationResult<IUserInfoResponse>>(new
                {
                    UserId = userId
                });

                var expection = new ArgumentNullException(nameof(DbUser));

                var expected = new
                {
                    IsSuccess = false,
                    Errors = new List<string> { expection.Message },
                    Body = null as object
                };

                SerializerAssert.AreEqual(expected, response.Message);
            }
            finally
            {
                await harness.Stop();
            }
        }*/
    }
}
