using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json.Serialization;
using LT.DigitalOffice.Kernel.Broker;
using System.Threading.Tasks;
using System.Text.Json;
using LT.DigitalOffice.Models.Broker.Responses.File;
using LT.DigitalOffice.Models.Broker.Requests.File;
using LT.DigitalOffice.Kernel.Enums;

namespace LT.DigitalOffice.UserService.Business.UnitTests
{
    public class EditUserCommandTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IPatchDbUserMapper> _mapperUserMock;
        private Mock<IAccessValidator> _accessValidatorMock;
        private Mock<IHttpContextAccessor> _httpAccessorMock;
        private Mock<ILogger<EditUserCommand>> _loggerMock;
        private Mock<IRequestClient<IAddImageRequest>> _rcImageMock;
        private Mock<IOperationResult<IAddImageResponse>> _operationResultAddImageMock;
        private Mock<Response<IOperationResult<IAddImageResponse>>> _responseBrokerAddImageMock;

        private JsonPatchDocument<DbUser> _patchDbUser;
        private JsonPatchDocument<EditUserRequest> _request;
        private IEditUserCommand _command;

        private Guid _adminId = Guid.NewGuid();
        private Guid _userId = Guid.NewGuid();
        private Guid _imageId = Guid.NewGuid();

        private void ClientRequestUp(Guid newGuid)
        {
            IDictionary<object, object> httpContextItems = new Dictionary<object, object>();

            httpContextItems.Add("UserId", newGuid);

            _httpAccessorMock
                .Setup(x => x.HttpContext.Items)
                .Returns(httpContextItems);
        }

        private void RcAddImageSetUp()
        {
            _operationResultAddImageMock = new Mock<IOperationResult<IAddImageResponse>>();
            _operationResultAddImageMock.Setup(x => x.Body.Id).Returns(_imageId);
            _operationResultAddImageMock.Setup(x => x.IsSuccess).Returns(true);
            _operationResultAddImageMock.Setup(x => x.Errors).Returns(new List<string>());

            _responseBrokerAddImageMock = new Mock<Response<IOperationResult<IAddImageResponse>>>();
            _responseBrokerAddImageMock
               .SetupGet(x => x.Message)
               .Returns(_operationResultAddImageMock.Object);

            _rcImageMock.Setup(
                x => x.GetResponse<IOperationResult<IAddImageResponse>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()))
                .Returns(Task.FromResult(_responseBrokerAddImageMock.Object));
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _request = new JsonPatchDocument<EditUserRequest>(new List<Operation<EditUserRequest>>
            {
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.FirstName)}",
                    "",
                    "Name"),
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.MiddleName)}",
                    "",
                    "Middlename"),
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.LastName)}",
                    "",
                    "Lastname"),
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.City)}",
                    "",
                    "City"),
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.DateOfBirth)}",
                    "",
                    "05/01/2009 14:57:32.8"),
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.Gender)}",
                    "",
                    UserGender.NotSelected),
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.Status)}",
                    "",
                    UserStatus.Vacation),
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.StartWorkingAt)}",
                    "",
                    "05/01/2009 14:57:32.8"),
                new Operation<EditUserRequest>(
                    "replace",
                    $"/{nameof(EditUserRequest.AvatarImage)}",
                    "",
                    JsonSerializer.Serialize(new AddImageRequest
                    {
                        Name = "Test",
                        Content = "[84][104][105][115][32]",
                        Extension = ".jpg"
                    }))
            }, new CamelCasePropertyNamesContractResolver());

            _patchDbUser = new JsonPatchDocument<DbUser>(new List<Operation<DbUser>>
            {
                new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.FirstName)}",
                    "",
                    "Name"),
                new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.MiddleName)}",
                    "",
                    "Middlename"),
                new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.LastName)}",
                    "",
                    "Lastname"),
                new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.City)}",
                    "",
                    "City"),
                new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.DateOfBirth)}",
                    "",
                    "05/01/2009 14:57:32.8"),
                new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.Gender)}",
                    "",
                    UserGender.NotSelected),
                new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.Status)}",
                    "",
                    UserStatus.Vacation),
                new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.StartWorkingAt)}",
                    "",
                    "05/01/2009 14:57:32.8"),
                new Operation<DbUser>(
                    "replace",
                    $"/{nameof(DbUser.AvatarFileId)}",
                    "",
                    _imageId)
            }, new CamelCasePropertyNamesContractResolver());

            _httpAccessorMock = new Mock<IHttpContextAccessor>();
        }

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();

            _mapperUserMock = new Mock<IPatchDbUserMapper>();

            _accessValidatorMock = new Mock<IAccessValidator>();

            _loggerMock = new Mock<ILogger<EditUserCommand>>();
            _rcImageMock = new Mock<IRequestClient<IAddImageRequest>>();

            ClientRequestUp(_adminId);

            _command = new EditUserCommand(
                _loggerMock.Object,
                _rcImageMock.Object,
                _httpAccessorMock.Object,
                _userRepositoryMock.Object,
                _mapperUserMock.Object,
                _accessValidatorMock.Object);

            _accessValidatorMock
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(true);

            _mapperUserMock
                .Setup(x => x.Map(
                    It.IsAny<JsonPatchDocument<EditUserRequest>>(), It.IsAny<Guid?>()))
                .Returns(_patchDbUser);

            _userRepositoryMock
                .Setup(x => x.Get(_adminId))
                .Returns(new DbUser { Id = _adminId, IsAdmin = true });

            _userRepositoryMock
                .Setup(x => x.EditUser(It.IsAny<Guid>(), It.IsAny<JsonPatchDocument<DbUser>>()))
                .Returns(true);

            RcAddImageSetUp();
        }

        [Test]
        public void ShouldThrowExceptionWhenEmailIsAlreadyTaken()
        {
            _userRepositoryMock
                .Setup(x => x.EditUser(It.IsAny<Guid>(), It.IsAny<JsonPatchDocument<DbUser>>()))
                .Throws(new Exception());

            Assert.Throws<Exception>(() => _command.Execute(_userId, _request));
            _userRepositoryMock.Verify(repository =>
                repository.EditUser(It.IsAny<Guid>(), It.IsAny<JsonPatchDocument<DbUser>>()), Times.Once);
            _rcImageMock.Verify(x =>
                x.GetResponse<IOperationResult<IAddImageResponse>>(It.IsAny<object>(), default, TimeSpan.FromSeconds(2)), Times.Once);
        }

        [Test]
        public void ShouldThrowExceptionWhenCurrentUserIsNotAdminAndNotHaveRights()
        {
            _accessValidatorMock
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(false);

            var userId = Guid.NewGuid();
            ClientRequestUp(userId);
            _userRepositoryMock
                .Setup(x => x.Get(userId))
                .Returns(new DbUser { Id = userId, IsAdmin = false });

            Assert.Throws<ForbiddenException>(() => _command.Execute(_userId, _request));
            _userRepositoryMock.Verify(repository =>
                repository.EditUser(It.IsAny<Guid>(), It.IsAny<JsonPatchDocument<DbUser>>()), Times.Never);
            _rcImageMock.Verify(x =>
                x.GetResponse<IOperationResult<IAddImageResponse>>(It.IsAny<object>(), default, TimeSpan.FromSeconds(2)), Times.Never);
        }

        [Test]
        public void ShouldEditUserWithoutImageWhenRequestIsNotSuccesful()
        {
            _operationResultAddImageMock = new Mock<IOperationResult<IAddImageResponse>>();
            _operationResultAddImageMock.Setup(x => x.IsSuccess).Returns(false);
            _operationResultAddImageMock.Setup(x => x.Errors).Returns(new List<string> { "error" });

            _responseBrokerAddImageMock = new Mock<Response<IOperationResult<IAddImageResponse>>>();
            _responseBrokerAddImageMock
               .SetupGet(x => x.Message)
               .Returns(_operationResultAddImageMock.Object);

            _rcImageMock.Setup(
                x => x.GetResponse<IOperationResult<IAddImageResponse>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()))
                .Returns(Task.FromResult(_responseBrokerAddImageMock.Object));

            Assert.AreEqual(OperationResultStatusType.PartialSuccess, _command.Execute(_userId, _request).Status);
            _userRepositoryMock.Verify(repository =>
                repository.EditUser(It.IsAny<Guid>(), It.IsAny<JsonPatchDocument<DbUser>>()), Times.Once);
            _rcImageMock.Verify(x =>
                x.GetResponse<IOperationResult<IAddImageResponse>>(It.IsAny<object>(), default, TimeSpan.FromSeconds(2)), Times.Once);
        }

        [Test]
        public void ShouldEditUserWhenUserDataIsValid()
        {
            Assert.AreEqual(_command.Execute(_userId, _request).Status, OperationResultStatusType.FullSuccess);
            _userRepositoryMock.Verify(repository =>
                repository.EditUser(It.IsAny<Guid>(), It.IsAny<JsonPatchDocument<DbUser>>()), Times.Once);
            _rcImageMock.Verify(x =>
                x.GetResponse<IOperationResult<IAddImageResponse>>(It.IsAny<object>(), default, TimeSpan.FromSeconds(2)), Times.Once);
        }
    }
}