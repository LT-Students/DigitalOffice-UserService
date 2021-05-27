using FluentValidation;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Business.Commands.Education;
using LT.DigitalOffice.UserService.Business.Commands.Education.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using LT.DigitalOffice.UserService.Models.Dto.Responses;
using LT.DigitalOffice.UserService.Validation.User.Interfaces.Education;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Moq;
using Moq.AutoMock;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Business.UnitTests.EducationsCommandTests
{
    class EditEducationCommandTests
    {
        private IEditEducationCommand _command;
        private AutoMocker _mocker;

        private JsonPatchDocument<EditEducationRequest> _request;
        private JsonPatchDocument<DbUserEducation> _dbRequest;

        private Guid _userId;
        private Guid _educationId;
        private DbUserEducation _dbUserEducation;
        private DbUser _dbUser;

        [SetUp]
        public void SetUp()
        {
            _mocker = new AutoMocker();
            _command = _mocker.CreateInstance<EditEducationCommand>();

            _userId = Guid.NewGuid();
            _educationId = Guid.NewGuid();

            _dbUserEducation = new DbUserEducation
            {
                Id = _educationId,
                UserId = _userId,
                UniversityName = "UniversityName",
                QualificationName = "QualificationName",
                AdmissionAt = DateTime.UtcNow,
                IssueAt = DateTime.UtcNow,
                FormEducation = 1
            };

            _dbUser = new DbUser
            {
                Id = _userId,
                IsAdmin = true
            };

            #region requests initialization

            var time = DateTime.UtcNow;

            _request = new JsonPatchDocument<EditEducationRequest>(
                new List<Operation<EditEducationRequest>>
                    {
                        new Operation<EditEducationRequest>(
                            "replace",
                            $"/{nameof(EditEducationRequest.UniversityName)}",
                            "",
                            "New University name"),
                        new Operation<EditEducationRequest>(
                            "replace",
                            $"/{nameof(EditEducationRequest.QualificationName)}",
                            "",
                            "New Qualification name"),
                        new Operation<EditEducationRequest>(
                            "replace",
                            $"/{nameof(EditEducationRequest.AdmissionAt)}",
                            "",
                            time),
                        new Operation<EditEducationRequest>(
                            "replace",
                            $"/{nameof(EditEducationRequest.IssueAt)}",
                            "",
                            time),
                        new Operation<EditEducationRequest>(
                            "replace",
                            $"/{nameof(EditEducationRequest.FormEducation)}",
                            "",
                            0)
                    }, new CamelCasePropertyNamesContractResolver()
                );

            _dbRequest = new JsonPatchDocument<DbUserEducation>(
                new List<Operation<DbUserEducation>>
                    {
                        new Operation<DbUserEducation>(
                            "replace",
                            $"/{nameof(DbUserEducation.UniversityName)}",
                            "",
                            "New University name"),
                        new Operation<DbUserEducation>(
                            "replace",
                            $"/{nameof(DbUserEducation.QualificationName)}",
                            "",
                            "New Qualification name"),
                        new Operation<DbUserEducation>(
                            "replace",
                            $"/{nameof(DbUserEducation.AdmissionAt)}",
                            "",
                            time),
                        new Operation<DbUserEducation>(
                            "replace",
                            $"/{nameof(DbUserEducation.IssueAt)}",
                            "",
                            time),
                        new Operation<DbUserEducation>(
                            "replace",
                            $"/{nameof(DbUserEducation.FormEducation)}",
                            "",
                            0)
                    }, new CamelCasePropertyNamesContractResolver());

            #endregion

            _mocker
                .Setup<IAccessValidator, bool>(x => x.IsAdmin(null))
                .Returns(true);

            IDictionary<object, object> _items = new Dictionary<object, object>();
            _items.Add("UserId", _dbUser.Id);

            _mocker
                .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
                .Returns(_items);

            _mocker
                .Setup<IPatchDbUserEducationMapper, JsonPatchDocument<DbUserEducation>>(x => x.Map(_request))
                .Returns(_dbRequest);

            _mocker
                .Setup<IEditEducationRequestValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            _mocker
                .Setup<IEducationRepository, bool>(x => x.Edit(It.IsAny<DbUserEducation>(), It.IsAny<JsonPatchDocument<DbUserEducation>>()))
                .Returns(true);

            _mocker
                .Setup<IEducationRepository, DbUserEducation>(x => x.Get(_educationId))
                .Returns(_dbUserEducation);

            _mocker
                .Setup<IUserRepository, DbUser>(x => x.Get(_dbUser.Id))
                .Returns(_dbUser);
        }

        [Test]
        public void ShouldThrowForbiddenExceptionWhenUserHasNotRight()
        {
            _mocker
                .Setup<IUserRepository, DbUser>(x => x.Get(_dbUser.Id))
                .Returns(new DbUser { IsAdmin = false });

            _mocker
                .Setup<IAccessValidator, bool>(x => x.HasRights(Rights.AddEditRemoveUsers))
                .Returns(false);

            _dbUserEducation.UserId = Guid.NewGuid();

            Assert.Throws<ForbiddenException>(() => _command.Execute(_educationId, _request));
            _mocker.Verify<IEducationRepository, bool>(x => x.Edit(It.IsAny<DbUserEducation>(), It.IsAny<JsonPatchDocument<DbUserEducation>>()),
                Times.Never);
            _mocker.Verify<IEducationRepository, DbUserEducation>(x => x.Get(_educationId),
                Times.Once);
            _mocker.Verify<IUserRepository, DbUser>(x => x.Get(_dbUser.Id),
                Times.Once);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenValidationInFailed()
        {
            _mocker
                .Setup<IEditEducationRequestValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(false);

            Assert.Throws<ValidationException>(() => _command.Execute(_educationId, _request));
            _mocker.Verify<IEducationRepository, bool>(x => x.Edit(It.IsAny<DbUserEducation>(), It.IsAny<JsonPatchDocument<DbUserEducation>>()),
                Times.Never);
            _mocker.Verify<IEducationRepository, DbUserEducation>(x => x.Get(_educationId),
                Times.Once);
            _mocker.Verify<IUserRepository, DbUser>(x => x.Get(_dbUser.Id),
                Times.Once);
        }

        [Test]
        public void ShouldThrowExceptionWhenRepositoryThrow()
        {
            _mocker
                .Setup<IUserRepository>(x => x.Get(_dbUser.Id))
                .Throws(new Exception());

            Assert.Throws<Exception>(() => _command.Execute(_educationId, _request));
            _mocker.Verify<IEducationRepository, bool>(x => x.Edit(_dbUserEducation, _dbRequest),
                Times.Never);
            _mocker.Verify<IEducationRepository, DbUserEducation>(x => x.Get(It.IsAny<Guid>()),
                Times.Never);
            _mocker.Verify<IUserRepository, DbUser>(x => x.Get(_dbUser.Id),
                Times.Once);
        }

        [Test]
        public void ShouldEditEducationSuccesfull()
        {
            var expectedResponse = new OperationResultResponse<bool>
            {
                Status = OperationResultStatusType.FullSuccess,
                Body = true
            };

            SerializerAssert.AreEqual(expectedResponse, _command.Execute(_educationId, _request));
            _mocker.Verify<IEducationRepository, bool>(x => x.Edit(_dbUserEducation, _dbRequest),
                Times.Once);
            _mocker.Verify<IEducationRepository, DbUserEducation>(x => x.Get(_educationId),
                Times.Once);
            _mocker.Verify<IUserRepository, DbUser>(x => x.Get(_dbUser.Id),
                Times.Once);
        }
    }
}
