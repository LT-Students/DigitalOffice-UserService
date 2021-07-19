using FluentValidation;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Business.Commands.Education;
using LT.DigitalOffice.UserService.Business.Commands.Education.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using LT.DigitalOffice.UserService.Validation.Education.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Business.UnitTests.EducationsCommandTests
{
    class CreateEducationCommandTests
    {
        private ICreateEducationCommand _command;
        private AutoMocker _mocker;

        private CreateEducationRequest _request;
        private DbUserEducation _dbEducation;
        private DbUser _dbUser;

        [SetUp]
        public void SetUp()
        {
            _mocker = new AutoMocker();
            _command = _mocker.CreateInstance<CreateEducationCommand>();

            _request = new CreateEducationRequest
            {
                UserId = Guid.NewGuid(),
                UniversityName = "name",
                QualificationName = "name",
                AdmissionAt = DateTime.UtcNow,
                IssueAt = DateTime.UtcNow,
                FormEducation = FormEducation.FullTime
            };

            _dbEducation = new DbUserEducation
            {
                Id = Guid.NewGuid(),
                UserId = _request.UserId,
                UniversityName = _request.UniversityName,
                QualificationName = _request.QualificationName,
                AdmissionAt = _request.AdmissionAt,
                IssueAt = _request.IssueAt,
                FormEducation = (int)_request.FormEducation
            };

            _dbUser = new DbUser
            {
                Id = Guid.NewGuid(),
                IsAdmin = true
            };

            IDictionary<object, object> _items = new Dictionary<object, object>();
            _items.Add("UserId", _dbUser.Id);

            _mocker
                .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
                .Returns(_items);

            _mocker
                .Setup<IDbUserEducationMapper, DbUserEducation>(x => x.Map(_request))
                .Returns(_dbEducation);

            _mocker
                .Setup<ICreateEducationRequestValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            _mocker
                .Setup<IUserRepository, DbUser>(x => x.Get(_dbUser.Id))
                .Returns(_dbUser);
        }

        [Test]
        public void ShouldThrowForbiddenExceptionWhenUserHasNotRight()
        {
            _mocker
                .Setup<IAccessValidator, bool>(x => x.HasRights(Rights.AddEditRemoveUsers))
                .Returns(false);

            _mocker
                .Setup<IUserRepository, DbUser>(x => x.Get(_dbUser.Id))
                .Returns(new DbUser { IsAdmin = false });

            Assert.Throws<ForbiddenException>(() => _command.Execute(_request));
            _mocker.Verify<IEducationRepository>(x => x.Add(It.IsAny<DbUserEducation>()), Times.Never);
            _mocker.Verify<IUserRepository>(x => x.Get(_dbUser.Id), Times.Once);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenValidationInFailed()
        {
            _mocker
                .Setup<ICreateEducationRequestValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(false);

            Assert.Throws<ValidationException>(() => _command.Execute(_request));
            _mocker.Verify<IEducationRepository>(x => x.Add(It.IsAny<DbUserEducation>()), Times.Never);
            _mocker.Verify<IUserRepository>(x => x.Get(_dbUser.Id), Times.Once);
        }

        [Test]
        public void ShouldThrowExceptionWhenRepositoryThrow()
        {
            _mocker
                .Setup<IUserRepository>(x => x.Get(It.IsAny<Guid>()))
                .Throws(new Exception());

            Assert.Throws<Exception>(() => _command.Execute(_request));
            _mocker.Verify<IEducationRepository>(x => x.Add(_dbEducation), Times.Never);
            _mocker.Verify<IUserRepository>(x => x.Get(_dbUser.Id), Times.Once);
        }

        [Test]
        public void ShouldAddEducationSuccesfull()
        {
            var expectedResponse = new OperationResultResponse<Guid>(
                _dbEducation.Id,
                OperationResultStatusType.FullSuccess,
                new List<string>()
            );

            SerializerAssert.AreEqual(expectedResponse, _command.Execute(_request));
            _mocker.Verify<IEducationRepository>(x => x.Add(_dbEducation), Times.Once);
            _mocker.Verify<IUserRepository>(x => x.Get(_dbUser.Id), Times.Once);
        }
    }
}
