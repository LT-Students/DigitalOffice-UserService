using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.UserService.Broker.Consumers;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using MassTransit;
using MassTransit.Testing;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.UnitTests.Consumers
{
    public class CreateAdminConsumerTests
    {
        private InMemoryTestHarness _harness;
        private ConsumerTestHarness<CreateAdminConsumer> _consumerTestHarness;
        private AutoMocker _mocker;
        private IRequestClient<ICreateAdminRequest> _requestClient;

        private object _request;
        private DbUser _user;

        [SetUp]
        public void SetUp()
        {
            _mocker = new AutoMocker();

            _harness = new InMemoryTestHarness();
            _consumerTestHarness = _harness.Consumer(() =>
                _mocker.CreateInstance<CreateAdminConsumer>());

            const string login = "admin";
            const string password = "password";
            const string firstName = "name";
            const string middleName = "middlename";
            const string lastName = "lastname";
            const string email = "email";

            _request = ICreateAdminRequest.CreateObj(
                firstName,
                middleName,
                lastName,
                email,
                login,
                password);

            _user = new()
            {
                Id = Guid.NewGuid(),
                FirstName = firstName,
                LastName = lastName,
                MiddleName = middleName
            };

            _mocker
                .Setup<IDbUserMapper, DbUser>(x => x.Map(It.IsAny<ICreateAdminRequest>()))
                .Returns(_user);
        }

        /*[Test]
        public async Task ShouldThrowExceptionWhenRepositoryThrow()
        {
            _mocker
                .Setup<IUserRepository>(x => x.Create(It.IsAny<DbUser>(), It.IsAny<DbUserCredentials>()))
                .Throws(new Exception());

            await _harness.Start();

            try
            {
                _requestClient = await _harness.ConnectRequestClient<ICreateAdminRequest>();

                var response = _requestClient.GetResponse<IOperationResult<bool>>(
                    _request).Result.Message;

                Assert.IsFalse(response.IsSuccess);
                Assert.IsFalse(response.Body);
                Assert.IsNotEmpty(response.Errors);
                Assert.True(_consumerTestHarness.Consumed.Select<ICreateAdminRequest>().Any());
                Assert.True(_harness.Sent.Select<IOperationResult<bool>>().Any());
                _mocker
                    .Verify<IUserRepository, Guid>(x => x.Create(_user, It.IsAny<DbUserCredentials>()), Times.Once);
            }
            finally
            {
                await _harness.Stop();
            }
        }*/

        /*[Test]
        public async Task ShouldCreateSMTPSuccessfully()
        {
            await _harness.Start();

            try
            {
                _requestClient = await _harness.ConnectRequestClient<ICreateAdminRequest>();

                var response = _requestClient.GetResponse<IOperationResult<bool>>(
                    _request).Result.Message;

                Assert.True(response.IsSuccess);
                Assert.True(response.Body);
                Assert.IsNull(response.Errors);
                Assert.True(_consumerTestHarness.Consumed.Select<ICreateAdminRequest>().Any());
                Assert.True(_harness.Sent.Select<IOperationResult<bool>>().Any());
                _mocker
                    .Verify<IUserRepository, Guid>(x => x.Create(_user, It.IsAny<DbUserCredentials>()), Times.Once);
            }
            finally
            {
                await _harness.Stop();
            }
        }*/
    }
}
