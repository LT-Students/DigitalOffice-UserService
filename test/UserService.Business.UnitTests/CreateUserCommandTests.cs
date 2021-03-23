using FluentValidation;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.RequestsMappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
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
        private Mock<IAccessValidator> accessValidatorMock;

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
                Status = UserStatus.Sick,
                Password = "Example",
                IsAdmin = false,
                Connections = new List<UserConnection>()
                {
                    new UserConnection()
                    {
                        Type = ConnectionType.Email,
                        Value = "Ex@mail.ru"
                    }
                },
                Skills = new List<string>() { "C#", "C/C++" }
            };

            dbUser = new DbUser
            {
                Id = userId,
                FirstName = "Example",
                LastName = "Example",
                MiddleName = "Example",
                Status = 1,
                IsAdmin = false,
                Connections = new List<DbConnection>
                {
                    new DbConnection()
                    {
                        Id = userId,
                        Type = (int)ConnectionType.Email,
                        Value = "Ex@mail.ru"
                    }
                }
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
            accessValidatorMock = new Mock<IAccessValidator>();

            command = new CreateUserCommand(
                userRepositoryMock.Object,
                validatorMock.Object,
                mapperUserMock.Object,
                accessValidatorMock.Object,
                mapperUserCredentialsMock.Object);

            accessValidatorMock
                .Setup(x => x.IsAdmin())
                .Returns(true);

            accessValidatorMock
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(true);

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
        }

        [Test]
        public void ShouldThrowExceptionWhenRepositoryThrowsException()
        {
            userRepositoryMock
                .Setup(x => x.CreateUser(It.IsAny<DbUser>(), It.IsAny<DbUserCredentials>()))
                .Throws(new Exception());

            Assert.Throws<Exception>(() => command.Execute(request));
        }

        [Test]
        public void ShouldCreateUserWhenUserDataIsValid()
        {
            Assert.That(command.Execute(request), Is.EqualTo(userId));
            mapperUserMock.Verify();
            mapperUserCredentialsMock.Verify();
            validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
            userRepositoryMock.Verify();
        }

        [Test]
        public void ShouldCreateUserWhenUserDataIsValidAndCurrentUserIsAdmin()
        {
            accessValidatorMock
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(false);

            Assert.That(command.Execute(request), Is.EqualTo(userId));
            mapperUserMock.Verify();
            mapperUserCredentialsMock.Verify();
            validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
            userRepositoryMock.Verify();
        }

        [Test]
        public void ShouldCreateUserWhenUserDataIsValidAndCurrentUserHasRights()
        {
            accessValidatorMock
                .Setup(x => x.IsAdmin())
                .Returns(false);

            Assert.That(command.Execute(request), Is.EqualTo(userId));
            mapperUserMock.Verify();
            mapperUserCredentialsMock.Verify();
            validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
            userRepositoryMock.Verify();
        }

        [Test]
        public void ShouldThrowExceptionWhenMapperThrowsException()
        {
            mapperUserMock
                .Setup(mapper => mapper.Map(It.IsAny<UserRequest>()))
                .Throws<Exception>().Verifiable();

            mapperUserCredentialsMock
                .Setup(mapper => mapper.Map(It.IsAny<UserRequest>()))
                .Throws<Exception>().Verifiable();

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

        [Test]
        public void ShouldThrowExceptionWhenMethodOfCreateSkillInRepositoryThrowsException()
        {
            userRepositoryMock
                .Setup(x => x.CreateSkill(It.IsAny<string>()))
                .Throws(new BadRequestException());

            Assert.Throws<BadRequestException>(() => command.Execute(request));
        }

        [Test]
        public void ShouldCreateUserWhenRepositoryCorrectCreatingSkills()
        {
            userRepositoryMock
                .Setup(x => x.CreateSkill(It.IsAny<string>()))
                .Returns(Guid.NewGuid())
                .Verifiable();

            Assert.That(command.Execute(request), Is.EqualTo(userId));
            mapperUserMock.Verify();
            mapperUserCredentialsMock.Verify();
            validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
            userRepositoryMock.Verify();
        }

        [Test]
        public void ShouldCreateUserWhenRepositoryHasThisSkills()
        {
            userRepositoryMock
                .Setup(x => x.CreateSkill(It.IsAny<string>()))
                .Returns(Guid.NewGuid())
                .Verifiable();

            Assert.That(command.Execute(request), Is.EqualTo(userId));
            mapperUserMock.Verify();
            mapperUserCredentialsMock.Verify();
            validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
            userRepositoryMock.Verify();
        }

        [Test]
        public void ShouldCreateUserWhenRepositoryDoesNotHaveThisSkills()
        {
            userRepositoryMock
                .Setup(x => x.FindSkillByName(It.IsAny<string>()))
                .Returns(() => null)
                .Verifiable();

            userRepositoryMock
                .Setup(x => x.CreateSkill(It.IsAny<string>()))
                .Returns(Guid.NewGuid())
                .Verifiable();

            Assert.That(command.Execute(request), Is.EqualTo(userId));
            mapperUserMock.Verify();
            mapperUserCredentialsMock.Verify();
            validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
            userRepositoryMock.Verify();
        }
    }
}