using FluentValidation;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.DbMappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Validation.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Business.UnitTests
{
    public class EditUserCommandTests
    {
        private Mock<IUserRepository> userRepositoryMock;
        private Mock<ICreateUserRequestValidator> validatorMock;
        private Mock<ValidationResult> validationResultIsValidMock;
        private Mock<IDbUserMapper> mapperUserMock;
        private Mock<IUserCredentialsRepository> userCredentialsRepositoryMock;
        private Mock<IAccessValidator> accessValidatorMock;
        private Mock<IDbUserCredentialsMapper> mapperUserCredentialsMock;

        private DbUser dbUser;
        private CreateUserRequest request;
        private IEditUserCommand command;
        private DbUserCredentials dbUserCredentials;
        private ValidationResult validationResultError;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            request = new CreateUserRequest
            {
                FirstName = "Example",
                LastName = "Example",
                MiddleName = "Example",
                Status = UserStatus.Sick,
                Password = "Example",
                IsAdmin = false
            };

            Guid userId = Guid.NewGuid();

            dbUser = new DbUser
            {
                Id = userId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                MiddleName = request.MiddleName,
                Status = (int)request.Status,
                IsAdmin = request.IsAdmin,
                IsActive = true
            };

            dbUserCredentials = new DbUserCredentials
            {
                UserId = userId,
                PasswordHash = request.Password,
                Salt = "Example"
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
            userCredentialsRepositoryMock = new Mock<IUserCredentialsRepository>();

            mapperUserMock = new Mock<IDbUserMapper>();
            mapperUserCredentialsMock = new Mock<IDbUserCredentialsMapper>();

            validatorMock = new Mock<ICreateUserRequestValidator>();
            accessValidatorMock = new Mock<IAccessValidator>();

            command = new EditUserCommand(validatorMock.Object,
                userRepositoryMock.Object,
                userCredentialsRepositoryMock.Object,
                mapperUserMock.Object,
                mapperUserCredentialsMock.Object,
                accessValidatorMock.Object);

            accessValidatorMock
                .Setup(x => x.IsAdmin())
                .Returns(true);

            accessValidatorMock
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(true);

            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
                .Returns(validationResultIsValidMock.Object);

            mapperUserMock
                .Setup(x => x.Map(It.IsAny<CreateUserRequest>(), It.IsAny<Func<string, string, string, string>>()))
                .Returns(dbUser);

            mapperUserCredentialsMock
                .Setup(x => x.Map(It.IsAny<CreateUserRequest>()))
                .Returns(dbUserCredentials);

            userRepositoryMock
                .Setup(x => x.EditUser(It.IsAny<DbUser>()))
                .Returns(true);

            userCredentialsRepositoryMock
                .Setup(x => x.EditUserCredentials(It.IsAny<DbUserCredentials>()))
                .Returns(true);
        }

        [Test]
        public void ShouldThrowExceptionWhenUserDataIsInvalid()
        {
            validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
                .Returns(validationResultError);

            Assert.Throws<ValidationException>(() => command.Execute(request));
            userRepositoryMock.Verify(repository => repository.EditUser(It.IsAny<DbUser>()), Times.Never);
        }

        [Test]
        public void ShouldThrowExceptionWhenEmailIsAlreadyTaken()
        {
            userRepositoryMock
                .Setup(x => x.EditUser(It.IsAny<DbUser>()))
                .Throws(new Exception());

            Assert.Throws<Exception>(() => command.Execute(request));
        }

        [Test]
        public void ShouldThrowExceptionWhenCurrentUserIsNotAdminAndNotHaveRights()
        {
            accessValidatorMock
                .Setup(x => x.IsAdmin())
                .Returns(false);

            accessValidatorMock
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(false);

            Assert.Throws<ForbiddenException>(() => command.Execute(request));
        }

        [Test]
        public void ShouldEditUserWhenUserDataIsValidAndCurrentUserHasRights()
        {
            accessValidatorMock
                .Setup(x => x.IsAdmin())
                .Returns(false);

            Assert.IsTrue(command.Execute(request));
            userRepositoryMock.Verify(repository => repository.EditUser(It.IsAny<DbUser>()), Times.Once);
        }

        [Test]
        public void ShouldEditUserWhenUserDataIsValidAndCurrentUserIsAdmin()
        {
            accessValidatorMock
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(false);

            Assert.IsTrue(command.Execute(request));
            userRepositoryMock.Verify(repository => repository.EditUser(It.IsAny<DbUser>()), Times.Once);
        }
    }
}