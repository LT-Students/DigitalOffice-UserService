using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Business.Commands.Education;
using LT.DigitalOffice.UserService.Business.Commands.Education.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
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
        private DbUser _dbUser;
        private DbUserEducation _dbUserEducation;

        [SetUp]
        public void SetUp()
        {
            _mocker = new AutoMocker();
            _command = _mocker.CreateInstance<RemoveEducationCommand>();

            _userId = Guid.NewGuid();
            _educationId = Guid.NewGuid();

            _dbUser = new DbUser
            {
                Id = _userId,
                IsAdmin = true
            };

            _dbUserEducation = new DbUserEducation
            {
                Id = _educationId,
                UserId = _dbUser.Id,
                IsActive = true
            };

            _mocker
                .Setup<IAccessValidator, bool>(x => x.IsAdmin(null))
                .Returns(true);

            IDictionary<object, object> _items = new Dictionary<object, object>();
            _items.Add("UserId", _dbUser.Id);

            _mocker
                .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
                .Returns(_items);

            _mocker
                .Setup<IEducationRepository, bool>(x => x.Remove(_dbUserEducation))
                .Returns(true);

            _mocker
                .Setup<IUserRepository, DbUser>(x => x.Get(_dbUser.Id))
                .Returns(_dbUser);

            _mocker
                .Setup<IEducationRepository, DbUserEducation>(x => x.Get(_educationId))
                .Returns(_dbUserEducation);
        }

        [Test]
        public void ShouldThrowForbiddenExceptionWhenUserHasNotRight()
        {
            var userId = Guid.NewGuid();

            IDictionary<object, object> _items = new Dictionary<object, object>();
            _items.Add("UserId", userId);

            _mocker
                .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
                .Returns(_items);

            _mocker
                .Setup<IUserRepository, DbUser>(x => x.Get(userId))
                .Returns(new DbUser { IsAdmin = false });

            _mocker
                .Setup<IAccessValidator, bool>(x => x.HasRights(Rights.AddEditRemoveUsers))
                .Returns(false);

            Assert.Throws<ForbiddenException>(() => _command.Execute(_educationId));
            _mocker.Verify<IEducationRepository, bool>(x => x.Remove(It.IsAny<DbUserEducation>()), Times.Never);
            _mocker.Verify<IUserRepository>(x => x.Get(userId), Times.Once);
            _mocker.Verify<IEducationRepository>(x => x.Get(_educationId), Times.Once);
        }

        [Test]
        public void ShouldThrowExceptionWhenRepositoryThrow()
        {
            _mocker
                .Setup<IUserRepository>(x => x.Get(It.IsAny<Guid>()))
                .Throws(new Exception());

            Assert.Throws<Exception>(() => _command.Execute(_educationId));
            _mocker.Verify<IEducationRepository, bool>(x => x.Remove(It.IsAny<DbUserEducation>()), Times.Never);
            _mocker.Verify<IUserRepository>(x => x.Get(_dbUser.Id), Times.Once);
            _mocker.Verify<IEducationRepository>(x => x.Get(_educationId), Times.Never);
        }

        [Test]
        public void ShouldRemoveEducationSuccesfull()
        {
            var expectedResponse = new OperationResultResponse<bool>
            {
                Status = OperationResultStatusType.FullSuccess,
                Body = true
            };

            SerializerAssert.AreEqual(expectedResponse, _command.Execute(_educationId));
            _mocker.Verify<IEducationRepository, bool>(x => x.Remove(_dbUserEducation), Times.Once);
            _mocker.Verify<IUserRepository>(x => x.Get(_dbUser.Id), Times.Once);
            _mocker.Verify<IEducationRepository>(x => x.Get(_educationId), Times.Once);
        }
    }
}
