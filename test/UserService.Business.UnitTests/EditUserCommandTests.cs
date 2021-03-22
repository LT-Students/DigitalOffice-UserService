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
    public class EditUserCommandTests
    {
        private Mock<IUserRepository> userRepositoryMock;
        private Mock<IValidator<UserRequest>> validatorMock;
        private Mock<ValidationResult> validationResultIsValidMock;
        private Mock<IUserRequestMapper> mapperUserMock;
        private Mock<IUserCredentialsRepository> userCredentialsRepositoryMock;
        private Mock<IAccessValidator> accessValidatorMock;
        private Mock<IUserCredentialsRequestMapper> mapperUserCredentialsMock;

        private DbUser dbUser;
        private UserRequest request;
        private IEditUserCommand command;
        private DbUserCredentials dbUserCredentials;
        private ValidationResult validationResultError;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            request = new UserRequest
            {
                Id = Guid.NewGuid(),
                FirstName = "Example",
                LastName = "Example",
                MiddleName = "Example",
                Email = "Example@gmail.com",
                Status = UserStatus.Sick,
                Password = "Example",
                IsAdmin = false,
                IsActive = true,
                AvatarFileId = Guid.NewGuid()
            };

            dbUser = new DbUser
            {
                Id = (Guid)request.Id,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                MiddleName = request.MiddleName,
                Status = (int)request.Status,
                IsAdmin = request.IsAdmin,
                IsActive = request.IsActive,
                AvatarFileId = request.AvatarFileId
            };

            dbUserCredentials = new DbUserCredentials
            {
                UserId = (Guid)request.Id,
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

            mapperUserMock = new Mock<IUserRequestMapper>();
            mapperUserCredentialsMock = new Mock<IUserCredentialsRequestMapper>();

            validatorMock = new Mock<IValidator<UserRequest>>();
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
                .Setup(x => x.Map(It.IsAny<UserRequest>()))
                .Returns(dbUser);

            mapperUserCredentialsMock
                .Setup(x => x.Map(It.IsAny<UserRequest>()))
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