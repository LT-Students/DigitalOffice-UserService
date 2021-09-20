using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Requests.File;
using LT.DigitalOffice.Models.Broker.Responses.File;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Business.Commands.Certificate;
using LT.DigitalOffice.UserService.Business.Commands.Certificate.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Certificates;
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
using System.Text.Json;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.UnitTests.CertificateCommandTests
{
    class EditCertificateCommandTests
    {
        private IEditCertificateCommand _command;
        private AutoMocker _mocker;

        private JsonPatchDocument<EditCertificateRequest> _request;
        private JsonPatchDocument<DbUserCertificate> _dbRequest;

        private Guid _userId;
        private Guid _certificateId;
        private DbUserCertificate _dbUserCertificate;
        private DbUser _dbUser;
        private Guid _imageId;

        private void RequestClientMock()
        {
            var _operationResultAddImageMock = new Mock<IOperationResult<Guid>>();
            _operationResultAddImageMock.Setup(x => x.Body).Returns(_imageId);
            _operationResultAddImageMock.Setup(x => x.IsSuccess).Returns(true);
            _operationResultAddImageMock.Setup(x => x.Errors).Returns(new List<string>());

            var responseBrokerAddImageMock = new Mock<Response<IOperationResult<Guid>>>();
            responseBrokerAddImageMock
               .SetupGet(x => x.Message)
               .Returns(_operationResultAddImageMock.Object);

            _mocker
                .Setup<IRequestClient<IAddImageRequest>, Task>(
                    x => x.GetResponse<IOperationResult<Guid>>(
                        It.IsAny<object>(), default, It.IsAny<RequestTimeout>()))
                .Returns(Task.FromResult(responseBrokerAddImageMock.Object));
        }


        [SetUp]
        public void SetUp()
        {
            _mocker = new AutoMocker();
            _command = _mocker.CreateInstance<EditCertificateCommand>();

            _imageId = Guid.NewGuid();
            _userId = Guid.NewGuid();
            _certificateId = Guid.NewGuid();

            _dbUserCertificate = new DbUserCertificate
            {
                Id = _certificateId,
                UserId = _userId,
                Name = "Name",
                SchoolName = "SchoolName",
                ReceivedAt = DateTime.UtcNow,
                ImageId = _imageId,
                EducationType = 1,
                IsActive = true
            };

            _dbUser = new DbUser
            {
                Id = _userId,
                IsAdmin = true
            };

            #region requests initialization

            var time = DateTime.UtcNow;

            _request = new JsonPatchDocument<EditCertificateRequest>(
                new List<Operation<EditCertificateRequest>> {
                    new Operation<EditCertificateRequest>
                    {
                        op = "replace",
                        path = $"/{nameof(EditCertificateRequest.Name)}",
                        value = "NewName"
                    },
                    new Operation<EditCertificateRequest>
                    {
                        op = "replace",
                        path = $"/{nameof(EditCertificateRequest.SchoolName)}",
                        value = "NewSchoolName"
                    },
                    new Operation<EditCertificateRequest>
                    {
                        op = "replace",
                        path = $"/{nameof(EditCertificateRequest.ReceivedAt)}",
                        value = time
                    },
                    new Operation<EditCertificateRequest>
                    {
                        op = "replace",
                        path = $"/{nameof(EditCertificateRequest.Image)}",
                        value = JsonSerializer.Serialize(new AddImageRequest
                        {
                            Name = "Test",
                            Content = "Content",
                            Extension = ".jpg"
                        }),
                    },
                    new Operation<EditCertificateRequest>
                    {
                        op = "replace",
                        path = $"/{nameof(EditCertificateRequest.IsActive)}",
                        value = false
                    },
                    new Operation<EditCertificateRequest>
                    {
                        op = "replace",
                        path = $"/{nameof(EditCertificateRequest.UserId)}",
                        value = _userId
                    }
                },
                new CamelCasePropertyNamesContractResolver());

            _dbRequest = new JsonPatchDocument<DbUserCertificate>(
                new List<Operation<DbUserCertificate>> {
                    new Operation<DbUserCertificate>
                    {
                        op = "replace",
                        path = $"/{nameof(DbUserCertificate.Name)}",
                        value = "NewName"
                    },
                    new Operation<DbUserCertificate>
                    {
                        op = "replace",
                        path = $"/{nameof(DbUserCertificate.SchoolName)}",
                        value = "NewSchoolName"
                    },
                    new Operation<DbUserCertificate>
                    {
                        op = "replace",
                        path = $"/{nameof(DbUserCertificate.ReceivedAt)}",
                        value = time
                    },
                    new Operation<DbUserCertificate>
                    {
                        op = "replace",
                        path = $"/{nameof(DbUserCertificate.ImageId)}",
                        value = _imageId
                    },
                    new Operation<DbUserCertificate>
                    {
                        op = "replace",
                        path = $"/{nameof(DbUserCertificate.IsActive)}",
                        value = false
                    },
                    new Operation<DbUserCertificate>
                    {
                        op = "replace",
                        path = $"/{nameof(DbUserCertificate.UserId)}",
                        value = _userId
                    }
                },
                new CamelCasePropertyNamesContractResolver());

            #endregion

            IDictionary<object, object> _items = new Dictionary<object, object>();
            _items.Add("UserId", _dbUser.Id);

            _mocker
                .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
                .Returns(_items);

            _mocker
                .Setup<IPatchDbUserCertificateMapper, JsonPatchDocument<DbUserCertificate>>(x => x.Map(_request, _imageId))
                .Returns(_dbRequest);

            _mocker
                .Setup<ICertificateRepository, bool>(x => x.Edit(It.IsAny<DbUserCertificate>(), It.IsAny<JsonPatchDocument<DbUserCertificate>>()))
                .Returns(true);

            _mocker
                .Setup<ICertificateRepository, DbUserCertificate>(x => x.Get(_certificateId))
                .Returns(_dbUserCertificate);

            _mocker
                .Setup<IUserRepository, DbUser>(x => x.Get(_dbUser.Id))
                .Returns(_dbUser);

            RequestClientMock();
        }

        /*[Test]
        public void ShouldThrowForbiddenExceptionWhenUserHasNotRight()
        {
            _mocker
                .Setup<IUserRepository, DbUser>(x => x.Get(_dbUser.Id))
                .Returns(new DbUser { IsAdmin = false });

            _mocker
                .Setup<IAccessValidator, bool>(x => x.HasRights(Rights.AddEditRemoveUsers))
                .Returns(false);

            _dbUserCertificate.UserId = Guid.NewGuid();

            Assert.Throws<ForbiddenException>(() => _command.Execute(_certificateId, _request));
            _mocker.Verify<ICertificateRepository, bool>(x => x.Edit(It.IsAny<DbUserCertificate>(), It.IsAny<JsonPatchDocument<DbUserCertificate>>()),
                Times.Never);
            _mocker.Verify<ICertificateRepository, DbUserCertificate>(x => x.Get(_certificateId),
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

            Assert.Throws<Exception>(() => _command.Execute(_certificateId, _request));
            _mocker.Verify<ICertificateRepository, bool>(x => x.Edit(_dbUserCertificate, _dbRequest),
                Times.Never);
            _mocker.Verify<ICertificateRepository, DbUserCertificate>(x => x.Get(It.IsAny<Guid>()),
                Times.Never);
            _mocker.Verify<IUserRepository, DbUser>(x => x.Get(_dbUser.Id),
                Times.Once);
        }

        *//*[Test]
        public void ShouldEditCertificateSuccesfull()
        {
            var expectedResponse = new OperationResultResponse<bool>
            {
                Status = OperationResultStatusType.FullSuccess,
                Body = true
            };

            SerializerAssert.AreEqual(expectedResponse, _command.Execute(_certificateId, _request));
            _mocker.Verify<ICertificateRepository, bool>(x => x.Edit(_dbUserCertificate, _dbRequest),
                Times.Once);
            _mocker.Verify<ICertificateRepository, DbUserCertificate>(x => x.Get(_certificateId),
                Times.Once);
            _mocker.Verify<IUserRepository, DbUser>(x => x.Get(_dbUser.Id),
                Times.Once);
        }*//*

        [Test]
        public void ShouldThrowExceptionWhenAddImageRequestThrow()
        {
            _mocker
               .Setup<IRequestClient<IAddImageRequest>, Task>(
                   x => x.GetResponse<IOperationResult<Guid>>(
                       It.IsAny<object>(), default, It.IsAny<RequestTimeout>()))
               .Throws(new Exception());

            var dbRequest = new JsonPatchDocument<DbUserCertificate>(
                new List<Operation<DbUserCertificate>> {
                    new Operation<DbUserCertificate>
                    {
                        op = "replace",
                        path = $"/{nameof(DbUserCertificate.Name)}",
                        value = "NewName"
                    },
                    new Operation<DbUserCertificate>
                    {
                        op = "replace",
                        path = $"/{nameof(DbUserCertificate.SchoolName)}",
                        value = "NewSchoolName"
                    },
                    new Operation<DbUserCertificate>
                    {
                        op = "replace",
                        path = $"/{nameof(DbUserCertificate.ReceivedAt)}",
                        value = DateTime.UtcNow
                    },
                    new Operation<DbUserCertificate>
                    {
                        op = "replace",
                        path = $"/{nameof(DbUserCertificate.ImageId)}",
                        value = null
                    },
                    new Operation<DbUserCertificate>
                    {
                        op = "replace",
                        path = $"/{nameof(DbUserCertificate.IsActive)}",
                        value = false
                    },
                    new Operation<DbUserCertificate>
                    {
                        op = "replace",
                        path = $"/{nameof(DbUserCertificate.UserId)}",
                        value = null
                    }
                },
                new CamelCasePropertyNamesContractResolver());

            _mocker
                .Setup<IPatchDbUserCertificateMapper, JsonPatchDocument<DbUserCertificate>>(x => x.Map(_request, null))
                .Returns(dbRequest);

            var expectedResponse = new OperationResultResponse<bool>
            {
                Status = OperationResultStatusType.PartialSuccess,
                Body = true,
                Errors = new List<string> { "Can not add certificate image to certificate. Please try again later." }
            };

            SerializerAssert.AreEqual(expectedResponse, _command.Execute(_certificateId, _request));
            _mocker.Verify<ICertificateRepository, bool>(x => x.Edit(_dbUserCertificate, dbRequest),
                Times.Once);
            _mocker.Verify<ICertificateRepository, DbUserCertificate>(x => x.Get(_certificateId),
                Times.Once);
            _mocker.Verify<IUserRepository, DbUser>(x => x.Get(_dbUser.Id),
                Times.Once);
        }

        [Test]
        public void ShouldThrowExceptionWhenAddImageRequestIsNotSuccessful()
        {
            var _operationResultAddImageMock = new Mock<IOperationResult<Guid>>();
            _operationResultAddImageMock.Setup(x => x.IsSuccess).Returns(false);
            _operationResultAddImageMock.Setup(x => x.Errors).Returns(new List<string>());

            var responseBrokerAddImageMock = new Mock<Response<IOperationResult<Guid>>>();
            responseBrokerAddImageMock
               .SetupGet(x => x.Message)
               .Returns(_operationResultAddImageMock.Object);

            _mocker
                .Setup<IRequestClient<IAddImageRequest>, Task>(
                    x => x.GetResponse<IOperationResult<Guid>>(
                        It.IsAny<object>(), default, It.IsAny<RequestTimeout>()))
                .Returns(Task.FromResult(responseBrokerAddImageMock.Object));

            var dbRequest = new JsonPatchDocument<DbUserCertificate>(
                new List<Operation<DbUserCertificate>> {
                    new Operation<DbUserCertificate>
                    {
                        op = "replace",
                        path = $"/{nameof(DbUserCertificate.Name)}",
                        value = "NewName"
                    },
                    new Operation<DbUserCertificate>
                    {
                        op = "replace",
                        path = $"/{nameof(DbUserCertificate.SchoolName)}",
                        value = "NewSchoolName"
                    },
                    new Operation<DbUserCertificate>
                    {
                        op = "replace",
                        path = $"/{nameof(DbUserCertificate.ReceivedAt)}",
                        value = DateTime.UtcNow
                    },
                    new Operation<DbUserCertificate>
                    {
                        op = "replace",
                        path = $"/{nameof(DbUserCertificate.ImageId)}",
                        value = null
                    },
                    new Operation<DbUserCertificate>
                    {
                        op = "replace",
                        path = $"/{nameof(DbUserCertificate.IsActive)}",
                        value = false
                    },
                    new Operation<DbUserCertificate>
                    {
                        op = "replace",
                        path = $"/{nameof(DbUserCertificate.UserId)}",
                        value = null
                    }
                },
                new CamelCasePropertyNamesContractResolver());

            _mocker
                .Setup<IPatchDbUserCertificateMapper, JsonPatchDocument<DbUserCertificate>>(x => x.Map(_request, null))
                .Returns(dbRequest);

            var expectedResponse = new OperationResultResponse<bool>
            {
                Status = OperationResultStatusType.PartialSuccess,
                Body = true,
                Errors = new List<string> { "Can not add certificate image to certificate. Please try again later." }
            };

            SerializerAssert.AreEqual(expectedResponse, _command.Execute(_certificateId, _request));
            _mocker.Verify<ICertificateRepository, bool>(x => x.Edit(_dbUserCertificate, dbRequest),
                Times.Once);
            _mocker.Verify<ICertificateRepository, DbUserCertificate>(x => x.Get(_certificateId),
                Times.Once);
            _mocker.Verify<IUserRepository, DbUser>(x => x.Get(_dbUser.Id),
                Times.Once);
        }*/
    }
}
