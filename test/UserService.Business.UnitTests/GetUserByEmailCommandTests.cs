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
    public class GetUserByEmailCommandTests
    {
        private Mock<IUserRepository> repositoryMock;
        private Mock<IMapper<DbUser, User>> mapperMock;
        private Mock<IValidator<string>> validatorMock;

        private IGetUserByEmailCommand command;

        private string userEmail;
        private User user;
        private DbUser dbUser;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            userEmail = "example@gmail.com";
            user = new User { Email = userEmail };

            dbUser = new DbUser
            {
                Id = Guid.NewGuid(),
                FirstName = "Example",
                LastName = "Example",
                MiddleName = "Example",
                Status = "Example",
                IsAdmin = false,
                IsActive = true,
                AvatarFileId = Guid.NewGuid()
            };
        }

        [SetUp]
        public void SetUp()
        {
            validatorMock = new Mock<IValidator<string>>();
            repositoryMock = new Mock<IUserRepository>();
            mapperMock = new Mock<IMapper<DbUser, User>>();

            command = new GetUserByEmailCommand(validatorMock.Object, repositoryMock.Object, mapperMock.Object);
        }

        [Test]
        public void ShouldThrowExceptionIfValidatorThrowsIt()
        {
            validatorMock
                .Setup(validator => validator.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(false);

            Assert.Throws<ValidationException>(() => command.Execute(userEmail));
            repositoryMock.Verify(repository => repository.GetUserByEmail(userEmail), Times.Never);
        }

        [Test]
        public void ShouldThrowExceptionAccordingToRepository()
        {
            validatorMock
                .Setup(validator => validator.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            repositoryMock
                .Setup(x => x.GetUserByEmail(It.IsAny<string>()))
                .Throws(new Exception());

            mapperMock
                .Setup(x => x.Map(It.IsAny<DbUser>()))
                .Returns(user);

            Assert.Throws<Exception>(() => command.Execute(userEmail));
            mapperMock.Verify(mapper => mapper.Map(It.IsAny<DbUser>()), Times.Never);
        }

        [Test]
        public void ShouldReturnsCorrectModelOfUserIfUserExists()
        {
            validatorMock
                .Setup(validator => validator.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            repositoryMock
                .Setup(repository => repository.GetUserByEmail(userEmail))
                .Returns(dbUser);

            mapperMock
                .Setup(mapper => mapper.Map(dbUser))
                .Returns(user);

            var result = command.Execute(userEmail);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<User>(result);
            Assert.AreEqual(userEmail, result.Email);
            repositoryMock.Verify(repository => repository.GetUserByEmail(userEmail), Times.Once);
        }
    }
}