using FluentValidation;
using FluentValidation.Results;
using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Kernel.Broker;
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
        private Mock<IUserRepository> repositoryMock;
        private Mock<IValidator<string>> validatorMock;
        private Mock<ValidationResult> validationResultIsValidMock;
        private Mock<IRequestClient<IUserDescriptionRequest>> requestClientMock;

        private IMemoryCache cache;
        private IOptions<CacheOptions> cacheOptions;

        private DbUser dbUser;
        private string userEmail;
        private IForgotPasswordCommand command;
        private ValidationResult validationResultError;
        private OperationResult<bool> operationResult;

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
                Status = 1,
                AvatarFileId = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = false
            };

            BrokerSetUp();

            command = new ForgotUserPasswordCommand(requestClientMock.Object,
                cacheOptions, validatorMock.Object, repositoryMock.Object, cache);

            validationResultError = new ValidationResult(
                new List<ValidationFailure>
                {
                    new ValidationFailure("error", "something", null)
                });

            validationResultIsValidMock = new Mock<ValidationResult>();

            validationResultIsValidMock
                .Setup(x => x.IsValid)
                .Returns(true);
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
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
                .Returns(validationResultError);

            Assert.Throws<ValidationException>(() => command.Execute(userEmail));
        }

        [Test]
        public void ShouldThrowExceptionWhenUserNotFound()
        {
            operationResult.IsSuccess = true;
            operationResult.Errors = new List<string>();
            operationResult.Body = true;

            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
                .Returns(validationResultIsValidMock.Object);

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
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
                .Returns(validationResultIsValidMock.Object);

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
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
                .Returns(validationResultIsValidMock.Object);

            repositoryMock
                .Setup(x => x.GetUserByEmail(It.IsAny<string>()))
                .Returns(dbUser);

            Assert.IsTrue(command.Execute(userEmail));
        }
    }
}