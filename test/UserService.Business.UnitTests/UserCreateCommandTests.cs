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
        private ICreateUserCommand command;
        private Mock<IValidator<CreateUserRequest>> validatorMock;
        private Mock<IMapper<CreateUserRequest, DbUser>> mapperMock;
        private Guid userId = Guid.NewGuid();

        private CreateUserRequest request = new CreateUserRequest
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
            mapperMock = new Mock<IMapper<CreateUserRequest, DbUser>>();
            validatorMock = new Mock<IValidator<CreateUserRequest>>();

            command = new CreateUserCommand(validatorMock.Object, repositoryMock.Object, mapperMock.Object);
        }

        [Test]
        public void ShouldThrowExceptionWhenRepositoryThrowsException()
        {
            validatorMock.Setup(validator => validator.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);
            repositoryMock
                .Setup(x => x.CreateUser(It.IsAny<DbUser>()))
                .Throws(new Exception());

            Assert.Throws<Exception>(() => command.Execute(request));
        }

        [Test]
        public void ShouldCreateUserWhenUserDataIsValid()
        {
            validatorMock.Setup(validator => validator.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);
            repositoryMock
                .Setup(x => x.CreateUser(It.IsAny<DbUser>()))
                .Returns(userId)
                .Verifiable();
            mapperMock
                .Setup(mapper => mapper.Map(It.IsAny<CreateUserRequest>()))
                .Returns(new DbUser())
                .Verifiable();

            Assert.That(command.Execute(request), Is.EqualTo(userId));
            mapperMock.Verify();
            validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
            repositoryMock.Verify();
        }

        [Test]
        public void ShouldThrowExceptionWhenMapperThrowsException()
        {
            validatorMock.Setup(validator => validator.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);
            repositoryMock
                .Setup(x => x.CreateUser(It.IsAny<DbUser>()))
                .Returns(userId);
            mapperMock.Setup(mapper => mapper.Map(It.IsAny<CreateUserRequest>())).Throws<Exception>().Verifiable();

            Assert.Throws<Exception>(() => command.Execute(request));
            validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
            mapperMock.Verify();
            repositoryMock.Verify(repository => repository.CreateUser(It.IsAny<DbUser>()), Times.Never);
        }

        /*[Test]
        public void ShouldThrowExceptionWhenValidatorThrowsException()
        {
            validatorMock.Setup(validator => validator.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(false);
            repositoryMock
                .Setup(x => x.UserCreate(It.IsAny<DbUser>()))
                .Returns(userId);
            mapperMock
                .Setup(mapper => mapper.Map(It.IsAny<UserCreateRequest>()))
                .Returns(new DbUser());

            Assert.Throws<ValidationException>(() => command.Execute(request));
            validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
            repositoryMock.Verify(repository => repository.UserCreate(It.IsAny<DbUser>()), Times.Never);
            mapperMock.Verify(mapper => mapper.Map(It.IsAny<UserCreateRequest>()), Times.Never);
        }*/
    }
}
