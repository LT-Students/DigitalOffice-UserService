﻿using FluentValidation;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using Moq;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.UserService.Business.UnitTests
{
    public class EditUserCommandTests
    {
        private Mock<IUserRepository> userRepositoryMock;
        private Mock<IUserCredentialsRepository> userCredentialsRepositoryMock;
        private Mock<IValidator<UserRequest>> validatorMock;
        private Mock<IMapper<UserRequest, DbUser>> mapperUserMock;
        private Mock<IMapper<UserRequest, DbUserCredentials>> mapperUserCredentialsMock;

        private IEditUserCommand command;
        private UserRequest request;
        private DbUserCredentials dbUserCredentials;
        private DbUser dbUser;

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
                Status = "Example",
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
                Status = request.Status,
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
        }

        [SetUp]
        public void SetUp()
        {
            userRepositoryMock = new Mock<IUserRepository>();
            userCredentialsRepositoryMock = new Mock<IUserCredentialsRepository>();

            mapperUserMock = new Mock<IMapper<UserRequest, DbUser>>();
            mapperUserCredentialsMock = new Mock<IMapper<UserRequest, DbUserCredentials>>();

            validatorMock = new Mock<IValidator<UserRequest>>();

            command = new EditUserCommand(validatorMock.Object,
                userRepositoryMock.Object,
                userCredentialsRepositoryMock.Object,
                mapperUserMock.Object,
                mapperUserCredentialsMock.Object);
        }

        [Test]
        public void ShouldThrowExceptionWhenUserDataIsInvalid()
        {
            validatorMock
                .Setup(validator => validator.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(false);

            Assert.Throws<ValidationException>(() => command.Execute(request));
            userRepositoryMock.Verify(repository => repository.EditUser(It.IsAny<DbUser>()), Times.Never);
        }

        [Test]
        public void ShouldThrowExceptionWhenEmailIsAlreadyTaken()
        {
            validatorMock
                .Setup(validator => validator.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            mapperUserMock
                .Setup(x => x.Map(It.IsAny<UserRequest>()))
                .Returns(dbUser);

            mapperUserCredentialsMock
                .Setup(x => x.Map(It.IsAny<UserRequest>()))
                .Returns(dbUserCredentials);

            userRepositoryMock
                .Setup(x => x.EditUser(It.IsAny<DbUser>()))
                .Throws(new Exception());

            Assert.Throws<Exception>(() => command.Execute(request));
        }

        [Test]
        public void ShouldEditUserWhenUserDataIsValid()
        {
            validatorMock
                .Setup(validator => validator.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

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

            Assert.IsTrue(command.Execute(request));
            userRepositoryMock.Verify(repository => repository.EditUser(It.IsAny<DbUser>()), Times.Once);
        }
    }
}