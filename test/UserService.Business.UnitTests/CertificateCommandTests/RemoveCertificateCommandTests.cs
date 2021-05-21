using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Business.Commands.Certificate;
using LT.DigitalOffice.UserService.Business.Commands.Certificate.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Responses;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Business.UnitTests.CertificateCommandTests
{
    class RemoveCertificateCommandTests
    {
        private IRemoveCertificateCommand _command;
        private AutoMocker _mocker;

        private Guid _userId;
        private Guid _certificateId;
        private DbUser _dbUser;
        private DbUserCertificate _dbUserCertificate;

        [SetUp]
        public void SetUp()
        {
            _mocker = new AutoMocker();
            _command = _mocker.CreateInstance<RemoveCertificateCommand>();

            _userId = Guid.NewGuid();
            _certificateId = Guid.NewGuid();

            _dbUser = new DbUser
            {
                Id = _userId,
                IsAdmin = true
            };

            _dbUserCertificate = new DbUserCertificate
            {
                Id = _certificateId,
                UserId = _dbUser.Id,
                IsActive = true
            };

            IDictionary<object, object> _items = new Dictionary<object, object>();
            _items.Add("UserId", _dbUser.Id);

            _mocker
                .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
                .Returns(_items);

            _mocker
                .Setup<IUserRepository, bool>(x => x.RemoveCertificate(_dbUserCertificate))
                .Returns(true);

            _mocker
                .Setup<IUserRepository, DbUser>(x => x.Get(_dbUser.Id))
                .Returns(_dbUser);

            _mocker
                .Setup<IUserRepository, DbUserCertificate>(x => x.GetCertificate(_certificateId))
                .Returns(_dbUserCertificate);
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

            Assert.Throws<ForbiddenException>(() => _command.Execute(_userId, _certificateId));
            _mocker.Verify<IUserRepository, bool>(x => x.RemoveCertificate(It.IsAny<DbUserCertificate>()), Times.Never);
            _mocker.Verify<IUserRepository>(x => x.Get(userId), Times.Once);
            _mocker.Verify<IUserRepository>(x => x.GetCertificate(_certificateId), Times.Never);
        }

        [Test]
        public void ShouldThrowExceptionWhenCertificateUserIdDontEqualUserIdFromRequest()
        {
            Assert.Throws<BadRequestException>(() => _command.Execute(Guid.NewGuid(), _certificateId));
            _mocker.Verify<IUserRepository, DbUserCertificate>(x => x.GetCertificate(_certificateId),
                Times.Once);
            _mocker.Verify<IUserRepository, bool>(x => x.RemoveCertificate(It.IsAny<DbUserCertificate>()),
                Times.Never);
            _mocker.Verify<IUserRepository, DbUser>(x => x.Get(_dbUser.Id),
                Times.Once);
        }

        [Test]
        public void ShouldThrowExceptionWhenRepositoryThrow()
        {
            _mocker
                .Setup<IUserRepository>(x => x.Get(It.IsAny<Guid>()))
                .Throws(new Exception());

            Assert.Throws<Exception>(() => _command.Execute(_userId, _certificateId));
            _mocker.Verify<IUserRepository, bool>(x => x.RemoveCertificate(It.IsAny<DbUserCertificate>()), Times.Never);
            _mocker.Verify<IUserRepository>(x => x.Get(_dbUser.Id), Times.Once);
            _mocker.Verify<IUserRepository>(x => x.GetEducation(_certificateId), Times.Never);
        }

        [Test]
        public void ShouldRemoveCertificateSuccesfull()
        {
            var expectedResponse = new OperationResultResponse<bool>
            {
                Status = OperationResultStatusType.FullSuccess,
                Body = true
            };

            SerializerAssert.AreEqual(expectedResponse, _command.Execute(_userId, _certificateId));
            _mocker.Verify<IUserRepository, bool>(x => x.RemoveCertificate(_dbUserCertificate), Times.Once);
            _mocker.Verify<IUserRepository>(x => x.Get(_dbUser.Id), Times.Once);
            _mocker.Verify<IUserRepository>(x => x.GetCertificate(_certificateId), Times.Once);
        }
    }
}
