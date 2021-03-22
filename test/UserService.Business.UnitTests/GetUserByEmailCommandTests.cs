using FluentValidation;
using FluentValidation.Results;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Business.UnitTests
{
    public class GetUserByEmailCommandTests
    {
        private Mock<IUserRepository> repositoryMock;
        private Mock<IUserResponseMapper> mapperMock;
        private Mock<IValidator<string>> validatorMock;
        private Mock<ValidationResult> validationResultIsValidMock;

        private IGetUserByEmailCommand command;

        private string userEmail;
        private User user;
        private DbUser dbUser;
        private ValidationResult validationResultError;

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
                Status = 1,
                IsAdmin = false,
                IsActive = true,
                AvatarFileId = Guid.NewGuid()
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
            validatorMock = new Mock<IValidator<string>>();
            repositoryMock = new Mock<IUserRepository>();
            mapperMock = new Mock<IUserResponseMapper>();

            command = new GetUserByEmailCommand(validatorMock.Object, repositoryMock.Object, mapperMock.Object);
        }

        [Test]
        public void ShouldThrowExceptionIfValidatorThrowsIt()
        {
            validatorMock
                .Setup(validator => validator.Validate(It.IsAny<IValidationContext>()))
                .Returns(validationResultError);

            Assert.Throws<ValidationException>(() => command.Execute(userEmail));
            repositoryMock.Verify(repository => repository.GetUserByEmail(userEmail), Times.Never);
        }

        [Test]
        public void ShouldThrowExceptionAccordingToRepository()
        {
            validatorMock
                .Setup(validator => validator.Validate(It.IsAny<IValidationContext>()))
                .Returns(validationResultIsValidMock.Object);

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
                .Setup(validator => validator.Validate(It.IsAny<IValidationContext>()))
                .Returns(validationResultIsValidMock.Object);

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