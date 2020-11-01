using FluentValidation;
using FluentValidation.Results;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Interfaces;
using LT.DigitalOffice.UserService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Business.UnitTests
{
    public class CreateUserCommandTests
    {
        private ICreateUserCommand command;
        private Mock<IUserRepository> userRepositoryMock;
        private Mock<IValidator<UserRequest>> validatorMock;
        private Mock<ValidationResult> validationResultIsValidMock;
        private Mock<IUserRequestMapper> mapperUserMock;
        private Mock<IUserCredentialsRequestMapper> mapperUserCredentialsMock;

        private Guid userId;
        private ValidationResult validationResultError;

        private UserRequest request;
        private DbUser dbUser;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            userId = Guid.NewGuid();

            request = new UserRequest
            {
                FirstName = "Example",
                LastName = "Example",
                MiddleName = "Example",
                Email = "Example@gmail.com",
                Status = "Example",
                Password = "Example",
                IsAdmin = false
            };

            dbUser = new DbUser
            {
                Id = userId,
                FirstName = "Example",
                LastName = "Example",
                MiddleName = "Example",
                Status = "Example",
                IsAdmin = false
            };

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

        [SetUp]
        public void SetUp()
        {
            userRepositoryMock = new Mock<IUserRepository>();

            mapperUserMock = new Mock<IUserRequestMapper>();
            mapperUserCredentialsMock = new Mock<IUserCredentialsRequestMapper>();

            validatorMock = new Mock<IValidator<UserRequest>>();

            command = new CreateUserCommand(
                userRepositoryMock.Object,
                validatorMock.Object,
                mapperUserMock.Object,
                mapperUserCredentialsMock.Object);
        }

        [Test]
        public void ShouldThrowExceptionWhenRepositoryThrowsException()
        {
            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
                .Returns(validationResultIsValidMock.Object);

            mapperUserMock
                .Setup(mapper => mapper.Map(It.IsAny<UserRequest>()))
                .Returns(new DbUser())
                .Verifiable();

            mapperUserCredentialsMock
                .Setup(mapper => mapper.Map(It.IsAny<UserRequest>()))
                .Returns(new DbUserCredentials())
                .Verifiable();

            userRepositoryMock
                .Setup(x => x.CreateUser(It.IsAny<DbUser>(), It.IsAny<DbUserCredentials>()))
                .Throws(new Exception());

            Assert.Throws<Exception>(() => command.Execute(request));
        }

        [Test]
        public void ShouldCreateUserWhenUserDataIsValid()
        {
            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
                .Returns(validationResultIsValidMock.Object);

            userRepositoryMock
                .Setup(x => x.CreateUser(It.IsAny<DbUser>(), It.IsAny<DbUserCredentials>()))
                .Returns(userId)
                .Verifiable();

            mapperUserMock
                .Setup(mapper => mapper.Map(It.IsAny<UserRequest>()))
                .Returns(dbUser)
                .Verifiable();

            mapperUserCredentialsMock
                .Setup(mapper => mapper.Map(It.IsAny<UserRequest>()))
                .Returns(new DbUserCredentials())
                .Verifiable();

            Assert.That(command.Execute(request), Is.EqualTo(userId));
            mapperUserMock.Verify();
            mapperUserCredentialsMock.Verify();
            validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
            userRepositoryMock.Verify();
        }

        [Test]
        public void ShouldThrowExceptionWhenMapperThrowsException()
        {
            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
                .Returns(validationResultIsValidMock.Object);

            mapperUserMock
                .Setup(mapper => mapper.Map(It.IsAny<UserRequest>()))
                .Throws<Exception>().Verifiable();

            mapperUserCredentialsMock
                .Setup(mapper => mapper.Map(It.IsAny<UserRequest>()))
                .Throws<Exception>().Verifiable();

            userRepositoryMock
                .Setup(x => x.CreateUser(It.IsAny<DbUser>(), It.IsAny<DbUserCredentials>()))
                .Returns(userId);

            Assert.Throws<Exception>(() => command.Execute(request));
            validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
            mapperUserMock.Verify();
            userRepositoryMock.Verify(
                repository => repository.CreateUser(It.IsAny<DbUser>(), It.IsAny<DbUserCredentials>()), Times.Never);
        }

        [Test]
        public void ShouldThrowExceptionWhenValidatorThrowsException()
        {
            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
                .Returns(validationResultError);

            Assert.Throws<ValidationException>(() => command.Execute(request));
            validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
        }
    }
}