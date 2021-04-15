using FluentValidation;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using Castle.Core.Logging;
using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Validation.User.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.UserService.Business.UnitTests
{
    public class EditUserCommandTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IEditUserRequestValidator> _validatorMock;
        private Mock<ValidationResult> _validationResultIsValidMock;
        private Mock<IPatchDbUserMapper> _mapperUserMock;
        private Mock<IAccessValidator> _accessValidatorMock;
        private Mock<IHttpContextAccessor> _httpAccessorMock;
        private Mock<ILogger<EditUserCommand>> _loggerMock;
        private Mock<IRequestClient<IAddImageRequest>> _rcImageMock;

        private JsonPatchDocument<DbUser> _patchDbUser;
        private JsonPatchDocument<EditUserRequest> _request;
        private IEditUserCommand _command;
        private ValidationResult _validationResultError;
        private Guid _userId = Guid.NewGuid();

        private void ClientRequestUp(Guid newGuid)
        {
            IDictionary<object, object> httpContextItems = new Dictionary<object, object>();

            httpContextItems.Add("UserId", newGuid);

            _httpAccessorMock
                .Setup(x => x.HttpContext.Items)
                .Returns(httpContextItems);
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _request = new JsonPatchDocument<EditUserRequest>();

            _patchDbUser = new JsonPatchDocument<DbUser>();

            _validationResultError = new ValidationResult(
                new List<ValidationFailure>
                {
                    new ValidationFailure("error", "something", null)
                });

            _validationResultIsValidMock = new Mock<ValidationResult>();

            _httpAccessorMock = new Mock<IHttpContextAccessor>();
        }

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();

            _mapperUserMock = new Mock<IPatchDbUserMapper>();

            _validatorMock = new Mock<IEditUserRequestValidator>();
            _accessValidatorMock = new Mock<IAccessValidator>();

            _loggerMock = new Mock<ILogger<EditUserCommand>>();
            _rcImageMock = new Mock<IRequestClient<IAddImageRequest>>();

            ClientRequestUp(_userId);

            _command = new EditUserCommand(
                _loggerMock.Object,
                _rcImageMock.Object,
                _httpAccessorMock.Object,
                _validatorMock.Object,
                _userRepositoryMock.Object,
                _mapperUserMock.Object,
                _accessValidatorMock.Object);

            _accessValidatorMock
                .Setup(x => x.IsAdmin())
                .Returns(true);

            _accessValidatorMock
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(true);

            _validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
                .Returns(_validationResultIsValidMock.Object);

            _mapperUserMock
                .Setup(x => x.Map(
                    It.IsAny<JsonPatchDocument<EditUserRequest>>(), It.IsAny<Func<string, Guid?>>()))
                .Returns(_patchDbUser);

            _userRepositoryMock
                .Setup(x => x.EditUser(It.IsAny<Guid>(), It.IsAny<JsonPatchDocument<DbUser>>()))
                .Returns(true);

            _validationResultIsValidMock
                .Setup(x => x.IsValid)
                .Returns(true);
        }

        [Test]
        public void ShouldThrowExceptionWhenUserDataIsInvalid()
        {
            _validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
                .Returns(_validationResultError);

            Assert.Throws<ValidationException>(() => _command.Execute(_userId, _request));
            _userRepositoryMock.Verify(repository =>
                repository.EditUser(It.IsAny<Guid>(), It.IsAny<JsonPatchDocument<DbUser>>()), Times.Never);
        }

        [Test]
        public void ShouldThrowExceptionWhenEmailIsAlreadyTaken()
        {
            _userRepositoryMock
                .Setup(x => x.EditUser(It.IsAny<Guid>(), It.IsAny<JsonPatchDocument<DbUser>>()))
                .Throws(new Exception());

            Assert.Throws<Exception>(() => _command.Execute(_userId, _request));
        }

        [Test]
        public void ShouldThrowExceptionWhenCurrentUserIsNotAdminAndNotHaveRights()
        {
            _accessValidatorMock
                .Setup(x => x.IsAdmin())
                .Returns(false);

            _accessValidatorMock
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(false);

            ClientRequestUp(Guid.NewGuid());

            Assert.Throws<ForbiddenException>(() => _command.Execute(_userId, _request));
        }

        [Test]
        public void ShouldEditUserWhenUserDataIsValidAndCurrentUserHasRights()
        {
            _accessValidatorMock
                .Setup(x => x.IsAdmin())
                .Returns(false);

            Assert.AreEqual(_command.Execute(_userId, _request).Status, OperationResultStatusType.FullSuccess);
            _userRepositoryMock.Verify(repository =>
                repository.EditUser(It.IsAny<Guid>(), It.IsAny<JsonPatchDocument<DbUser>>()), Times.Once);
        }

        [Test]
        public void ShouldEditUserWhenUserDataIsValidAndCurrentUserIsAdmin()
        {
            _accessValidatorMock
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(false);

            Assert.AreEqual(_command.Execute(_userId, _request).Status, OperationResultStatusType.FullSuccess);
            _userRepositoryMock.Verify(repository =>
                repository.EditUser(It.IsAny<Guid>(), It.IsAny<JsonPatchDocument<DbUser>>()), Times.Once);
        }
    }
}