using FluentValidation;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Business.Commands.User.Education;
using LT.DigitalOffice.UserService.Business.Commands.User.Interfaces.Education;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using LT.DigitalOffice.UserService.Models.Dto.Responses;
using LT.DigitalOffice.UserService.Validation.User.Interfaces.Education;
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

        [SetUp]
        public void SetUp()
        {
            _mocker = new AutoMocker();
            _command = _mocker.CreateInstance<CreateEducationCommand>();

            _mocker
                .Setup<IAccessValidator, bool>(x => x.IsAdmin(null))
                .Returns(true);

            _request = new CreateEducationRequest
            {
                UserId = Guid.NewGuid(),
                UniversityName = "name",
                QualificationName = "name",
                AdmissiomAt = DateTime.UtcNow,
                IssueAt = DateTime.UtcNow,
                FormEducation = FormEducation.FullTime
            };

            _dbEducation = new DbUserEducation
            {
                Id = Guid.NewGuid(),
                UserId = _request.UserId,
                UniversityName = _request.UniversityName,
                QualificationName = _request.QualificationName,
                AdmissiomAt = _request.AdmissiomAt,
                IssueAt = _request.IssueAt,
                FormEducation = (int)_request.FormEducation
            };

            IDictionary<object, object> _items = new Dictionary<object, object>();
            _items.Add("UserId", Guid.NewGuid());

            _mocker
                .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
                .Returns(_items);

            _mocker
                .Setup<IDbUserEducationMapper, DbUserEducation>(x => x.Map(_request))
                .Returns(_dbEducation);

            _mocker
                .Setup<IAccessValidator, bool>(x => x.IsAdmin(null))
                .Returns(true);

            _mocker
                .Setup<ICreateEducationRequestValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
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

            Assert.Throws<ForbiddenException>(() => _command.Execute(_request));
            _mocker.Verify<IUserRepository>(x => x.AddEducation(It.IsAny<DbUserEducation>()), Times.Never);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenValidationInFailed()
        {
            _mocker
                .Setup<ICreateEducationRequestValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(false);

            Assert.Throws<ValidationException>(() => _command.Execute(_request));
            _mocker.Verify<IUserRepository>(x => x.AddEducation(It.IsAny<DbUserEducation>()), Times.Never);
        }

        [Test]
        public void ShouldThrowExceptionWhenRepositoryThrow()
        {
            _mocker
                .Setup<IUserRepository>(x => x.AddEducation(_dbEducation))
                .Throws(new Exception());

            Assert.Throws<Exception>(() => _command.Execute(_request));
            _mocker.Verify<IUserRepository>(x => x.AddEducation(_dbEducation), Times.Once);
        }

        [Test]
        public void ShouldAddEducationSuccesfull()
        {
            var expectedResponse = new OperationResultResponse<Guid>
            {
                Status = OperationResultStatusType.FullSuccess,
                Body = _dbEducation.Id
            };

            SerializerAssert.AreEqual(expectedResponse, _command.Execute(_request));
            _mocker.Verify<IUserRepository>(x => x.AddEducation(_dbEducation), Times.Once);
        }
    }
}
