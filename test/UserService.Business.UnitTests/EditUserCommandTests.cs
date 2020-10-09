using FluentValidation;
using Moq;
using NUnit.Framework;
using System;
using LT.DigitalOffice.UserService.Mappers.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Business.Interfaces;

namespace LT.DigitalOffice.UserService.Business.UnitTests
{
    public class EditUserCommandTests
    {
        private Mock<IUserRepository> repositoryMock;
        private Mock<IValidator<EditUserRequest>> validatorMock;
        private Mock<IMapper<EditUserRequest, DbUser>> mapperMock;

        private IEditUserCommand command;
        private EditUserRequest request;
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
                Email = request.Email,
                Status = request.Status,
                PasswordHash = request.Password,
                IsAdmin = request.IsAdmin,
                IsActive = request.IsActive,
                AvatarFileId = request.AvatarFileId
            };
        }

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IUserRepository>();
            mapperMock = new Mock<IMapper<EditUserRequest, DbUser>>();
            validatorMock = new Mock<IValidator<EditUserRequest>>();

            command = new EditUserCommand(validatorMock.Object, repositoryMock.Object, mapperMock.Object);
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

            mapperMock
                .Setup(x => x.Map(It.IsAny<EditUserRequest>()))
                .Returns(dbUser);

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

            mapperMock
                .Setup(x => x.Map(It.IsAny<EditUserRequest>()))
                .Returns(dbUser);

            repositoryMock
                .Setup(x => x.EditUser(It.IsAny<DbUser>()))
                .Returns(true);

            Assert.IsTrue(command.Execute(request));
            repositoryMock.Verify(repository => repository.EditUser(It.IsAny<DbUser>()), Times.Once);
        }
    }
}