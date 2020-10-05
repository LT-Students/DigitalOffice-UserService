using FluentValidation;
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
        private Mock<IUserRepository> repositoryMock;
        private Mock<IValidator<EditUserRequest>> validatorMock;
        private Mock<IMapper<EditUserRequest, DbUser>> mapperUserMock;
        private Mock<IMapper<EditUserRequest, DbUserCredentials>> mapperUserCredentialsMock;

        private IEditUserCommand command;
        private EditUserRequest request;
        private DbUserCredentials dbUserCredentials;
        private DbUser dbUser;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            request = new EditUserRequest
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
                Id = request.Id,
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
                UserId = request.Id,
                PasswordHash = request.Password,
                Email = request.Email,
                Salt = "Example"
            };
        }

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IUserRepository>();

            mapperUserMock = new Mock<IMapper<EditUserRequest, DbUser>>();
            mapperUserCredentialsMock = new Mock<IMapper<EditUserRequest, DbUserCredentials>>();

            validatorMock = new Mock<IValidator<EditUserRequest>>();

            command = new EditUserCommand(validatorMock.Object, repositoryMock.Object,
                mapperUserMock.Object, mapperUserCredentialsMock.Object);
        }

        [Test]
        public void ShouldThrowExceptionWhenUserDataIsInvalid()
        {
            validatorMock
                .Setup(validator => validator.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(false);

            Assert.Throws<ValidationException>(() => command.Execute(request));
            repositoryMock.Verify(repository => repository.EditUser(It.IsAny<DbUser>()), Times.Never);
        }

        [Test]
        public void ShouldThrowExceptionWhenEmailIsAlreadyTaken()
        {
            validatorMock
                .Setup(validator => validator.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            mapperUserMock
                .Setup(x => x.Map(It.IsAny<EditUserRequest>()))
                .Returns(dbUser);

            mapperUserCredentialsMock
                .Setup(x => x.Map(It.IsAny<EditUserRequest>()))
                .Returns(dbUserCredentials);

            repositoryMock
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
                .Setup(x => x.Map(It.IsAny<EditUserRequest>()))
                .Returns(dbUser);

            mapperUserCredentialsMock
                .Setup(x => x.Map(It.IsAny<EditUserRequest>()))
                .Returns(dbUserCredentials);

            repositoryMock
                .Setup(x => x.EditUser(It.IsAny<DbUser>()))
                .Returns(true);

            repositoryMock
                .Setup(x => x.EditUserCredentials(It.IsAny<DbUserCredentials>()))
                .Returns(true);

            Assert.IsTrue(command.Execute(request));
            repositoryMock.Verify(repository => repository.EditUser(It.IsAny<DbUser>()), Times.Once);
        }
    }
}