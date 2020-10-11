using FluentValidation;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.UserService.Broker.Requests;
using LT.DigitalOffice.UserService.Business.Cache.Options;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using MassTransit;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.UnitTests
{
    public class OperationResult<T> : IOperationResult<T>
    {
        public bool IsSuccess { get; set; }

        public List<string> Errors { get; set; }

        public T Body { get; set; }
    }

    class ForgotUserPasswordCommandTests
    {
        private Mock<IRequestClient<IUserDescriptionRequest>> requestClientMock;
        private Mock<IValidator<string>> validatorMock;
        private Mock<IUserRepository> repositoryMock;
        private IOptions<CacheOptions> cacheOptions;
        private IMemoryCache cache;

        private IForgotPasswordCommand command;
        private OperationResult<bool> operationResult;
        private string userEmail;
        private DbUser dbUser;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            userEmail = "Example@gmail.com";

            cacheOptions = Options.Create(new CacheOptions()
            {
                CacheLiveInMinutes = 5
            });

            validatorMock = new Mock<IValidator<string>>();
            repositoryMock = new Mock<IUserRepository>();
            cache = new MemoryCache(new MemoryCacheOptions());

            dbUser = new DbUser
            {
                Id = Guid.NewGuid(),
                Email = userEmail,
                FirstName = "Example1",
                LastName = "Example1",
                MiddleName = "Example1",
                Status = "normal",
                AvatarFileId = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = false
            };

            BrokerSetUp();

            command = new ForgotUserPasswordCommand(requestClientMock.Object,
                cacheOptions, validatorMock.Object, repositoryMock.Object, cache);
        }

        private void BrokerSetUp()
        {
            var responseClientMock = new Mock<Response<IOperationResult<bool>>>();
            requestClientMock = new Mock<IRequestClient<IUserDescriptionRequest>>();

            operationResult = new OperationResult<bool>();

            requestClientMock.Setup(
                x => x.GetResponse<IOperationResult<bool>>(
                    It.IsAny<object>(), default, default))
                .Returns(Task.FromResult(responseClientMock.Object));

            responseClientMock
                .SetupGet(x => x.Message)
                .Returns(operationResult);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenUserEmailIsNotValid()
        {
            operationResult.IsSuccess = true;
            operationResult.Errors = new List<string>();
            operationResult.Body = true;

            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(false);

            Assert.Throws<ValidationException>(() => command.Execute(userEmail));
        }

        [Test]
        public void ShouldThrowExceptionWhenUserNotFound()
        {
            operationResult.IsSuccess = true;
            operationResult.Errors = new List<string>();
            operationResult.Body = true;

            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            repositoryMock
                .Setup(x => x.GetUserByEmail(It.IsAny<string>()))
                .Throws(new Exception());

            Assert.Throws<Exception>(() => command.Execute(userEmail));
        }

        [Test]
        public void ShouldThrowExceptionWhenBrokerResponseIsNotSuccess()
        {
            operationResult.IsSuccess = false;
            operationResult.Errors = new List<string>() { "Any errors"};
            operationResult.Body = false;

            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            repositoryMock
                .Setup(x => x.GetUserByEmail(It.IsAny<string>()))
                .Throws(new Exception());

            Assert.Throws<Exception>(() => command.Execute(userEmail));
        }

        [Test]
        public void ShouldSetGuidInCacheSuccessful()
        {
            operationResult.IsSuccess = true;
            operationResult.Errors = new List<string>();
            operationResult.Body = true;

            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            repositoryMock
                .Setup(x => x.GetUserByEmail(It.IsAny<string>()))
                .Returns(dbUser);

            Assert.IsTrue(command.Execute(userEmail));
        }
    }
}