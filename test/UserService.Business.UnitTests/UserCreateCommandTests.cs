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
    public class UserCreateCommandTests
    {
        private Mock<IUserRepository> repositoryMock;
        private IUserCreateCommand command;
        private Mock<IValidator<UserRequest>> validatorMock;
        private Mock<IMapper<UserRequest, DbUser>> mapperUserMock;
        private Mock<IMapper<UserRequest, DbUserCredentials>> mapperUserCredentialsMock;

        private Guid userId = Guid.NewGuid();

        private UserRequest request = new UserRequest
        {
            FirstName = "Example",
            LastName = "Example",
            MiddleName = "Example",
            Email = "Example@gmail.com",
            Status = "Example",
            Password = "Example",
            IsAdmin = false
        };

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IUserRepository>();

            mapperUserMock = new Mock<IMapper<UserRequest, DbUser>>();
            mapperUserCredentialsMock = new Mock<IMapper<UserRequest, DbUserCredentials>>();

            validatorMock = new Mock<IValidator<UserRequest>>();

            command = new UserCreateCommand(repositoryMock.Object, validatorMock.Object,
                mapperUserMock.Object, mapperUserCredentialsMock.Object);
        }

        [Test]
        public void ShouldThrowExceptionWhenRepositoryThrowsException()
        {
            validatorMock.Setup(validator => validator.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            mapperUserMock
                .Setup(mapper => mapper.Map(It.IsAny<UserRequest>()))
                .Returns(new DbUser())
                .Verifiable();

            mapperUserCredentialsMock
                .Setup(mapper => mapper.Map(It.IsAny<UserRequest>()))
                .Returns(new DbUserCredentials())
                .Verifiable();

            repositoryMock
                .Setup(x => x.UserCreate(It.IsAny<DbUser>(), It.IsAny<string>()))
                .Throws(new Exception());

            Assert.Throws<Exception>(() => command.Execute(request));
        }

        [Test]
        public void ShouldCreateUserWhenUserDataIsValid()
        {
            validatorMock.Setup(validator => validator.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);
            repositoryMock
                .Setup(x => x.UserCreate(It.IsAny<DbUser>(), It.IsAny<string>()))
                .Returns(userId)
                .Verifiable();

            mapperUserMock
                .Setup(mapper => mapper.Map(It.IsAny<UserRequest>()))
                .Returns(new DbUser())
                .Verifiable();

            mapperUserCredentialsMock
                .Setup(mapper => mapper.Map(It.IsAny<UserRequest>()))
                .Returns(new DbUserCredentials())
                .Verifiable();

            Assert.That(command.Execute(request), Is.EqualTo(userId));
            mapperUserMock.Verify();
            mapperUserCredentialsMock.Verify();
            validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
            repositoryMock.Verify();
        }

        [Test]
        public void ShouldThrowExceptionWhenMapperThrowsException()
        {
            validatorMock.Setup(validator => validator.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            mapperUserMock
                .Setup(mapper => mapper.Map(It.IsAny<UserRequest>()))
                .Throws<Exception>().Verifiable();
            mapperUserCredentialsMock
                .Setup(mapper => mapper.Map(It.IsAny<UserRequest>()))
                .Throws<Exception>().Verifiable();

            repositoryMock
                .Setup(x => x.UserCreate(It.IsAny<DbUser>(), It.IsAny<string>()))
                .Returns(userId);

            Assert.Throws<Exception>(() => command.Execute(request));
            validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
            mapperUserMock.Verify();
            repositoryMock.Verify(repository => repository.UserCreate(It.IsAny<DbUser>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void ShouldThrowExceptionWhenValidatorThrowsException()
        {
            validatorMock.Setup(validator => validator.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(false);

            Assert.Throws<ValidationException>(() => command.Execute(request));
            validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
        }
    }
}