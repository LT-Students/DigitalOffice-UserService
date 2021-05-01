using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Business.Commands.User;
using LT.DigitalOffice.UserService.Business.Commands.User.Interfaces.Education;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Responses;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Business.UnitTests.EducationsCommandTests
{
    class RemoveEducationCommandTests
    {
        private IRemoveEducationCommand _command;
        private AutoMocker _mocker;

        private Guid _userId;
        private Guid _educationId;

        [SetUp]
        public void SetUp()
        {
            _mocker = new AutoMocker();
            _command = _mocker.CreateInstance<RemoveEducationCommand>();

            _userId = Guid.NewGuid();
            _educationId = Guid.NewGuid();

            _mocker
                .Setup<IAccessValidator, bool>(x => x.IsAdmin(null))
                .Returns(true);

            IDictionary<object, object> _items = new Dictionary<object, object>();
            _items.Add("UserId", Guid.NewGuid());

            _mocker
                .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
                .Returns(_items);

            _mocker
                .Setup<IAccessValidator, bool>(x => x.IsAdmin(null))
                .Returns(true);

            _mocker
                .Setup<IUserRepository, bool>(x => x.RemoveEducation(It.IsAny<Guid>()))
                .Returns(true);
        }

        [Test]
        public void ShouldThrowForbiddenExceptionWhenUserHasNotRight()
        {
            _mocker
                .Setup<IAccessValidator, bool>(x => x.IsAdmin(null))
                .Returns(false);

            _mocker
                .Setup<IAccessValidator, bool>(x => x.HasRights(Rights.AddEditRemoveUsers))
                .Returns(false);

            Assert.Throws<ForbiddenException>(() => _command.Execute(_userId, _educationId));
            _mocker.Verify<IUserRepository, bool>(x => x.RemoveEducation(It.IsAny<Guid>()), Times.Never);
        }

        [Test]
        public void ShouldThrowExceptionWhenRepositoryThrow()
        {
            _mocker
                .Setup<IUserRepository>(x => x.RemoveEducation(_educationId))
                .Throws(new Exception());

            Assert.Throws<Exception>(() => _command.Execute(_userId, _educationId));
            _mocker.Verify<IUserRepository, bool>(x => x.RemoveEducation(_educationId), Times.Once);
        }

        [Test]
        public void ShouldRemoveEducationSuccesfull()
        {
            var expectedResponse = new OperationResultResponse<bool>
            {
                Status = OperationResultStatusType.FullSuccess,
                Body = true
            };

            SerializerAssert.AreEqual(expectedResponse, _command.Execute(_userId, _educationId));
            _mocker.Verify<IUserRepository, bool>(x => x.RemoveEducation(_educationId), Times.Once);
        }
    }
}
