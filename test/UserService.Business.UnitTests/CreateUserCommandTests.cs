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
    public class CreateUserCommandTests
    {
        private ICreateUserCommand command;
        private Mock<IUserRepository> userRepositoryMock;
        private Mock<ICreateUserRequestValidator> validatorMock;
        private Mock<ValidationResult> validationResultIsValidMock;
        private Mock<IDbUserMapper> mapperUserMock;
        private Mock<IDbUserCredentialsMapper> mapperUserCredentialsMock;
        private Mock<IAccessValidator> accessValidatorMock;

        private Guid userId;
        private ValidationResult validationResultError;

        private CreateUserRequest request;
        private DbUser dbUser;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            userId = Guid.NewGuid();

            request = new CreateUserRequest
            {
                FirstName = "Example",
                LastName = "Example",
                MiddleName = "Example",
                Status = UserStatus.Sick,
                Password = "Example",
                IsAdmin = false,
                Communications = new List<Communications>()
                {
                    new Communications()
                    {
                        Type = CommunicationType.Email,
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
                Communications = new List<DbUserCommunication>
                {
                    new DbUserCommunication()
                    {
                        Id = userId,
                        Type = (int)CommunicationType.Email,
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

            mapperUserMock = new Mock<IDbUserMapper>();
            mapperUserCredentialsMock = new Mock<IDbUserCredentialsMapper>();

            validatorMock = new Mock<ICreateUserRequestValidator>();
            accessValidatorMock = new Mock<IAccessValidator>();

            command = new CreateUserCommand(
                userRepositoryMock.Object,
                validatorMock.Object,
                mapperUserMock.Object,
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

            userRepositoryMock
                .Setup(x => x.CreateUser(It.IsAny<DbUser>()))
                .Returns(userId)
                .Verifiable();

            mapperUserMock
                .Setup(mapper => mapper.Map(It.IsAny<CreateUserRequest>(), It.IsAny<Func<string, string, string, string>>()))
                .Returns(dbUser)
                .Verifiable();

            mapperUserCredentialsMock
                .Setup(mapper => mapper.Map(It.IsAny<CreateUserRequest>()))
                .Returns(new DbUserCredentials())
                .Verifiable();
        }

        //[Test]
        //public void ShouldThrowExceptionWhenRepositoryThrowsException()
        //{
        //    userRepositoryMock
        //        .Setup(x => x.CreateUser(It.IsAny<DbUser>()))
        //        .Throws(new Exception());

        //    Assert.Throws<Exception>(() => command.Execute(request));
        //}

        //[Test]
        //public void ShouldCreateUserWhenUserDataIsValid()
        //{
        //    Assert.That(command.Execute(request), Is.EqualTo(userId));
        //    mapperUserMock.Verify();
        //    mapperUserCredentialsMock.Verify();
        //    validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
        //    userRepositoryMock.Verify();
        //}

        //[Test]
        //public void ShouldCreateUserWhenUserDataIsValidAndCurrentUserIsAdmin()
        //{
        //    accessValidatorMock
        //        .Setup(x => x.HasRights(It.IsAny<int>()))
        //        .Returns(false);

        //    Assert.That(command.Execute(request), Is.EqualTo(userId));
        //    mapperUserMock.Verify();
        //    mapperUserCredentialsMock.Verify();
        //    validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
        //    userRepositoryMock.Verify();
        //}

        //[Test]
        //public void ShouldCreateUserWhenUserDataIsValidAndCurrentUserHasRights()
        //{
        //    accessValidatorMock
        //        .Setup(x => x.IsAdmin())
        //        .Returns(false);

        //    Assert.That(command.Execute(request), Is.EqualTo(userId));
        //    mapperUserMock.Verify();
        //    mapperUserCredentialsMock.Verify();
        //    validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
        //    userRepositoryMock.Verify();
        //}

        //[Test]
        //public void ShouldThrowExceptionWhenMapperThrowsException()
        //{
        //    mapperUserMock
        //        .Setup(mapper => mapper.Map(It.IsAny<CreateUserRequest>(), It.IsAny<Func<string, string, string, string>>()))
        //        .Throws<Exception>().Verifiable();

        //    mapperUserCredentialsMock
        //        .Setup(mapper => mapper.Map(It.IsAny<CreateUserRequest>()))
        //        .Throws<Exception>().Verifiable();

        //    Assert.Throws<Exception>(() => command.Execute(request));
        //    validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
        //    mapperUserMock.Verify();
        //    userRepositoryMock.Verify(
        //        repository => repository.CreateUser(It.IsAny<DbUser>()), Times.Never);
        //}

        //[Test]
        //public void ShouldThrowExceptionWhenValidatorThrowsException()
        //{
        //    validatorMock
        //        .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
        //        .Returns(validationResultError);

        //    Assert.Throws<ValidationException>(() => command.Execute(request));
        //    validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
        //}

        //[Test]
        //public void ShouldThrowExceptionWhenMethodOfCreateSkillInRepositoryThrowsException()
        //{
        //    userRepositoryMock
        //        .Setup(x => x.CreateSkill(It.IsAny<string>()))
        //        .Throws(new BadRequestException());

        //    Assert.Throws<BadRequestException>(() => command.Execute(request));
        //}

        //[Test]
        //public void ShouldCreateUserWhenRepositoryCorrectCreatingSkills()
        //{
        //    userRepositoryMock
        //        .Setup(x => x.CreateSkill(It.IsAny<string>()))
        //        .Returns(Guid.NewGuid())
        //        .Verifiable();

        //    Assert.That(command.Execute(request), Is.EqualTo(userId));
        //    mapperUserMock.Verify();
        //    mapperUserCredentialsMock.Verify();
        //    validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
        //    userRepositoryMock.Verify();
        //}

        //[Test]
        //public void ShouldCreateUserWhenRepositoryHasThisSkills()
        //{
        //    userRepositoryMock
        //        .Setup(x => x.CreateSkill(It.IsAny<string>()))
        //        .Returns(Guid.NewGuid())
        //        .Verifiable();

        //    Assert.That(command.Execute(request), Is.EqualTo(userId));
        //    mapperUserMock.Verify();
        //    mapperUserCredentialsMock.Verify();
        //    validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
        //    userRepositoryMock.Verify();
        //}

        //[Test]
        //public void ShouldCreateUserWhenRepositoryDoesNotHaveThisSkills()
        //{
        //    userRepositoryMock
        //        .Setup(x => x.FindSkillByName(It.IsAny<string>()))
        //        .Returns(() => null)
        //        .Verifiable();

        //    userRepositoryMock
        //        .Setup(x => x.CreateSkill(It.IsAny<string>()))
        //        .Returns(Guid.NewGuid())
        //        .Verifiable();

        //    Assert.That(command.Execute(request), Is.EqualTo(userId));
        //    mapperUserMock.Verify();
        //    mapperUserCredentialsMock.Verify();
        //    validatorMock.Verify(validator => validator.Validate(It.IsAny<IValidationContext>()), Times.Once);
        //    userRepositoryMock.Verify();
        //}
    }
}