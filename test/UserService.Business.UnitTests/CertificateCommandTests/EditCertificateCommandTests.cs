using FluentValidation;
using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Business.Commands.Education;
using LT.DigitalOffice.UserService.Business.Commands.Education.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using LT.DigitalOffice.UserService.Models.Dto.Responses;
using LT.DigitalOffice.UserService.Validation.User.Interfaces.Education;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Moq;
using Moq.AutoMock;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.UnitTests.CertificateCommandTests
{
    class EditCertificateCommandTests
    {
        private IEditEducationCommand _command;
        private AutoMocker _mocker;

        private JsonPatchDocument<EditEducationRequest> _request;
        private JsonPatchDocument<DbUserCertificate> _dbRequest;

        private Guid _userId;
        private Guid _certificateId;
        private DbUserCertificate _dbUserCertificate;
        private DbUser _dbUser;
        private Guid _imageId;
        private AddImageRequest _image;

        private void RequestClientMock()
        {
            var _operationResultAddImageMock = new Mock<IOperationResult<IAddImageResponse>>();
            _operationResultAddImageMock.Setup(x => x.Body.Id).Returns(_imageId);
            _operationResultAddImageMock.Setup(x => x.IsSuccess).Returns(true);
            _operationResultAddImageMock.Setup(x => x.Errors).Returns(new List<string>());

            var responseBrokerAddImageMock = new Mock<Response<IOperationResult<IAddImageResponse>>>();
            responseBrokerAddImageMock
               .SetupGet(x => x.Message)
               .Returns(_operationResultAddImageMock.Object);

            _mocker
                .Setup<IRequestClient<IAddImageRequest>, Task>(
                    x => x.GetResponse<IOperationResult<IAddImageResponse>>(
                        It.IsAny<object>(), default, It.IsAny<RequestTimeout>()))
                .Returns(Task.FromResult(responseBrokerAddImageMock.Object));
        }


        [SetUp]
        public void SetUp()
        {
            _mocker = new AutoMocker();
            _command = _mocker.CreateInstance<EditEducationCommand>();

            _userId = Guid.NewGuid();
            _certificateId = Guid.NewGuid();

            _dbUserCertificate = new DbUserCertificate
            {
                Id = _certificateId,
                UserId = _userId,
                UniversityName = "UniversityName",
                QualificationName = "QualificationName",
                AdmissionAt = DateTime.UtcNow,
                IssueAt = DateTime.UtcNow,
                FormEducation = 1
            };

            _dbUser = new DbUser
            {
                Id = Guid.NewGuid(),
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

            _dbRequest = new JsonPatchDocument<DbUserCertificate>(
                new List<Operation<DbUserCertificate>>
                    {
                        new Operation<DbUserCertificate>(
                            "replace",
                            $"/{nameof(DbUserCertificate.UniversityName)}",
                            "",
                            "New University name"),
                        new Operation<DbUserCertificate>(
                            "replace",
                            $"/{nameof(DbUserCertificate.QualificationName)}",
                            "",
                            "New Qualification name"),
                        new Operation<DbUserCertificate>(
                            "replace",
                            $"/{nameof(DbUserCertificate.AdmissionAt)}",
                            "",
                            time),
                        new Operation<DbUserCertificate>(
                            "replace",
                            $"/{nameof(DbUserCertificate.IssueAt)}",
                            "",
                            time),
                        new Operation<DbUserCertificate>(
                            "replace",
                            $"/{nameof(DbUserCertificate.FormEducation)}",
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
                .Setup<IPatchDbUserEducationMapper, JsonPatchDocument<DbUserCertificate>>(x => x.Map(_request))
                .Returns(_dbRequest);

            _mocker
                .Setup<IEditEducationRequestValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            _mocker
                .Setup<IUserRepository, bool>(x => x.EditEducation(It.IsAny<DbUserCertificate>(), It.IsAny<JsonPatchDocument<DbUserCertificate>>()))
                .Returns(true);

            _mocker
                .Setup<IUserRepository, DbUserCertificate>(x => x.GetEducation(_certificateId))
                .Returns(_dbUserCertificate);

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

            Assert.Throws<ForbiddenException>(() => _command.Execute(_userId, _certificateId, _request));
            _mocker.Verify<IUserRepository, bool>(x => x.EditEducation(It.IsAny<DbUserCertificate>(), It.IsAny<JsonPatchDocument<DbUserCertificate>>()),
                Times.Never);
            _mocker.Verify<IUserRepository, DbUserCertificate>(x => x.GetEducation(It.IsAny<Guid>()),
                Times.Never);
            _mocker.Verify<IUserRepository, DbUser>(x => x.Get(_dbUser.Id),
                Times.Once);
        }

        [Test]
        public void ShouldThrowValidationExceptionWhenValidationInFailed()
        {
            _mocker
                .Setup<IEditEducationRequestValidator, bool>(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(false);

            Assert.Throws<ValidationException>(() => _command.Execute(_userId, _certificateId, _request));
            _mocker.Verify<IUserRepository, bool>(x => x.EditEducation(It.IsAny<DbUserCertificate>(), It.IsAny<JsonPatchDocument<DbUserCertificate>>()),
                Times.Never);
            _mocker.Verify<IUserRepository, DbUserCertificate>(x => x.GetEducation(It.IsAny<Guid>()),
                Times.Never);
            _mocker.Verify<IUserRepository, DbUser>(x => x.Get(_dbUser.Id),
                Times.Once);
        }

        [Test]
        public void ShouldThrowExceptionWhenEducationUserIdDontEqualUserIdFromRequest()
        {
            Assert.Throws<BadRequestException>(() => _command.Execute(Guid.NewGuid(), _certificateId, _request));
            _mocker.Verify<IUserRepository, DbUserCertificate>(x => x.GetEducation(_certificateId),
                Times.Once);
            _mocker.Verify<IUserRepository, bool>(x => x.EditEducation(_dbUserCertificate, _dbRequest),
                Times.Never);
            _mocker.Verify<IUserRepository, DbUser>(x => x.Get(_dbUser.Id),
                Times.Once);
        }

        [Test]
        public void ShouldThrowExceptionWhenRepositoryThrow()
        {
            _mocker
                .Setup<IUserRepository>(x => x.Get(_dbUser.Id))
                .Throws(new Exception());

            Assert.Throws<Exception>(() => _command.Execute(_userId, _certificateId, _request));
            _mocker.Verify<IUserRepository, bool>(x => x.EditEducation(_dbUserCertificate, _dbRequest),
                Times.Never);
            _mocker.Verify<IUserRepository, DbUserCertificate>(x => x.GetEducation(It.IsAny<Guid>()),
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

            SerializerAssert.AreEqual(expectedResponse, _command.Execute(_userId, _certificateId, _request));
            _mocker.Verify<IUserRepository, bool>(x => x.EditEducation(_dbUserCertificate, _dbRequest),
                Times.Once);
            _mocker.Verify<IUserRepository, DbUserCertificate>(x => x.GetEducation(_certificateId),
                Times.Once);
            _mocker.Verify<IUserRepository, DbUser>(x => x.Get(_dbUser.Id),
                Times.Once);
        }
    }
}
