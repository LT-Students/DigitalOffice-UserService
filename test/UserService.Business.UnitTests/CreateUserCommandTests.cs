using FluentValidation;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Models.Broker.Requests.File;
using LT.DigitalOffice.Models.Broker.Requests.Message;
using LT.DigitalOffice.Models.Broker.Responses.File;
using LT.DigitalOffice.Models.Broker.Responses.Message;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Models.Dto.Responses;
using LT.DigitalOffice.UserService.Validation.User.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.UnitTests
{
    class CreateUserCommandTests
    {
        private Mock<IDbUserMapper> _mapperUserMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IAccessValidator> _accessValidatorMock;
        private Mock<ILogger<CreateUserCommand>> _loggerMock;
        private Mock<ICreateUserRequestValidator> _validatorMock;
        private Mock<IRequestClient<IAddImageRequest>> _rcImageMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IRequestClient<ISendEmailRequest>> _rcSendEmailMock;
        private Mock<IRequestClient<IChangeUserPositionRequest>> _rcPositionMock;
        private Mock<IRequestClient<IChangeUserDepartmentRequest>> _rcDepartmentMock;
        private Mock<IRequestClient<IGetEmailTemplateTagsRequest>> _rcGetTemplateTagsMock;

        private Mock<IOperationResult<IAddImageResponse>> _operationResultAddImageMock;
        private Mock<IOperationResult<bool>> _operationResultSendEmailMock;
        private Mock<IOperationResult<bool>> _operationResultChangePositionMock;
        private Mock<IOperationResult<bool>> _operationResultChangeDepartmentMock;
        private Mock<IOperationResult<IGetEmailTemplateTagsResponse>> _operationResultGetTempTagsMock;

        private Guid _imageId = Guid.NewGuid();
        private DbUser _dbUser;
        private ICreateUserCommand _command;
        private CreateUserRequest _createUserRequest;
        private DbUserCommunication _dbCommunication;
        private OperationResultResponse<Guid> _expectedOperationResultResponse;

        #region Broker setup
        private void RcAddImageSetUp()
        {
            _operationResultAddImageMock = new Mock<IOperationResult<IAddImageResponse>>();
            _operationResultAddImageMock.Setup(x => x.Body.Id).Returns(_imageId);
            _operationResultAddImageMock.Setup(x => x.IsSuccess).Returns(true);
            _operationResultAddImageMock.Setup(x => x.Errors).Returns(new List<string>());

            var responseBrokerAddImageMock = new Mock<Response<IOperationResult<IAddImageResponse>>>();
            responseBrokerAddImageMock
               .SetupGet(x => x.Message)
               .Returns(_operationResultAddImageMock.Object);

            _rcImageMock.Setup(
                x => x.GetResponse<IOperationResult<IAddImageResponse>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()))
                .Returns(Task.FromResult(responseBrokerAddImageMock.Object));
        }

        private void RcGetTemplateTagSetUp()
        {
            var templateTags = new Dictionary<string, string>();
            templateTags.Add("userFirstName", "");
            templateTags.Add("userId", "");
            templateTags.Add("userPassword", "");
            templateTags.Add("userEmail", "");

            var tempateTagsResponse = new Mock<IGetEmailTemplateTagsResponse>();
            tempateTagsResponse.Setup(x => x.TemplateId).Returns(Guid.NewGuid());
            tempateTagsResponse.Setup(x => x.TemplateTags).Returns(templateTags);

            _operationResultGetTempTagsMock = new Mock<IOperationResult<IGetEmailTemplateTagsResponse>>();
            _operationResultGetTempTagsMock.Setup(x => x.Body).Returns(tempateTagsResponse.Object);
            _operationResultGetTempTagsMock.Setup(x => x.IsSuccess).Returns(true);
            _operationResultGetTempTagsMock.Setup(x => x.Errors).Returns(new List<string>());

            var responseBrokerGetTempTagsMock = new Mock<Response<IOperationResult<IGetEmailTemplateTagsResponse>>>();

            responseBrokerGetTempTagsMock
               .SetupGet(x => x.Message)
               .Returns(_operationResultGetTempTagsMock.Object);

            _rcGetTemplateTagsMock.Setup(
               x => x.GetResponse<IOperationResult<IGetEmailTemplateTagsResponse>>(
                   It.IsAny<object>(), default, It.IsAny<RequestTimeout>()))
               .Returns(Task.FromResult(responseBrokerGetTempTagsMock.Object));
        }

        private void RcSendEmailSetUp()
        {
            _operationResultSendEmailMock = new Mock<IOperationResult<bool>>();
            _operationResultSendEmailMock.Setup(x => x.Body).Returns(true);
            _operationResultSendEmailMock.Setup(x => x.IsSuccess).Returns(true);
            _operationResultSendEmailMock.Setup(x => x.Errors).Returns(new List<string>());

            var responseBrokerSendEmailMock = new Mock<Response<IOperationResult<bool>>>();

            responseBrokerSendEmailMock
               .SetupGet(x => x.Message)
               .Returns(_operationResultSendEmailMock.Object);

            _rcSendEmailMock.Setup(
                x => x.GetResponse<IOperationResult<bool>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()))
                .Returns(Task.FromResult(responseBrokerSendEmailMock.Object));
        }

        private void RcChangePositionSetUp()
        {
            _operationResultChangePositionMock = new Mock<IOperationResult<bool>>();
            _operationResultChangePositionMock.Setup(x => x.Body).Returns(true);
            _operationResultChangePositionMock.Setup(x => x.IsSuccess).Returns(true);
            _operationResultChangePositionMock.Setup(x => x.Errors).Returns(new List<string>());

            var responseBrokerChangePositionMock = new Mock<Response<IOperationResult<bool>>>();

            responseBrokerChangePositionMock
               .SetupGet(x => x.Message)
               .Returns(_operationResultChangePositionMock.Object);

            _rcPositionMock.Setup(
                x => x.GetResponse<IOperationResult<bool>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()))
                .Returns(Task.FromResult(responseBrokerChangePositionMock.Object));
        }

        private void RcChangeDepartmentSetUp()
        {
            _operationResultChangeDepartmentMock = new Mock<IOperationResult<bool>>();
            _operationResultChangeDepartmentMock.Setup(x => x.Body).Returns(true);
            _operationResultChangeDepartmentMock.Setup(x => x.IsSuccess).Returns(true);
            _operationResultChangeDepartmentMock.Setup(x => x.Errors).Returns(new List<string>());

            var responseBrokerChangeDepartmentMock = new Mock<Response<IOperationResult<bool>>>();

            responseBrokerChangeDepartmentMock
               .SetupGet(x => x.Message)
               .Returns(_operationResultChangeDepartmentMock.Object);

            _rcDepartmentMock.Setup(
                x => x.GetResponse<IOperationResult<bool>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()))
                .Returns(Task.FromResult(responseBrokerChangeDepartmentMock.Object));
        }
        #endregion

        #region Setup

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mapperUserMock = new Mock<IDbUserMapper>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _accessValidatorMock = new Mock<IAccessValidator>();
            _loggerMock = new Mock<ILogger<CreateUserCommand>>();
            _validatorMock = new Mock<ICreateUserRequestValidator>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            _rcImageMock = new Mock<IRequestClient<IAddImageRequest>>();
            _rcSendEmailMock = new Mock<IRequestClient<ISendEmailRequest>>();
            _rcPositionMock = new Mock<IRequestClient<IChangeUserPositionRequest>>();
            _rcDepartmentMock = new Mock<IRequestClient<IChangeUserDepartmentRequest>>();
            _rcGetTemplateTagsMock = new Mock<IRequestClient<IGetEmailTemplateTagsRequest>>();

            var userId = Guid.NewGuid();

            _createUserRequest = new CreateUserRequest
            {
                FirstName = "Ivan",
                LastName = "Ivanov",
                MiddleName = "Ivanovich",
                Status = UserStatus.Vacation,
                Password = "12341234",
                AvatarImage = new AddImageRequest
                {
                    Name = "name",
                    Content = "[84][104][105][115][32]",
                    Extension = ".jpg"
                },
                StartWorkingAt = "2021-08-23",
                IsAdmin = false,
                DepartmentId = Guid.NewGuid(),
                PositionId = Guid.NewGuid()
            };

            _dbCommunication = new DbUserCommunication
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = (int)CommunicationType.Email,
                Value = "IvanoIvan@gmail.com"
            };

            _dbUser = new DbUser
            {
                Id = userId,
                FirstName = _createUserRequest.FirstName,
                LastName = _createUserRequest.LastName,
                MiddleName = _createUserRequest.MiddleName,
                Status = (int)_createUserRequest.Status,
                AvatarFileId = _imageId,
                IsAdmin = (bool)_createUserRequest.IsAdmin,
                IsActive = true,
                Communications = new List<DbUserCommunication>
                {
                    _dbCommunication
                }
            };

            IDictionary<object, object> httpContextItems = new Dictionary<object, object>();

            httpContextItems.Add("UserId", userId);

            _httpContextAccessorMock
                .Setup(x => x.HttpContext.Items)
                .Returns(httpContextItems);

            _command = new CreateUserCommand(
                _loggerMock.Object,
                _rcImageMock.Object,
                _httpContextAccessorMock.Object,
                _rcGetTemplateTagsMock.Object,
                _rcDepartmentMock.Object,
                _rcPositionMock.Object,
                _rcSendEmailMock.Object,
                _userRepositoryMock.Object,
                _validatorMock.Object,
                _mapperUserMock.Object,
                _accessValidatorMock.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _expectedOperationResultResponse = new OperationResultResponse<Guid>()
            {
                Body = _dbUser.Id,
                Status = OperationResultStatusType.FullSuccess,
                Errors = new List<string>()
            };

            _mapperUserMock.Reset();
            _userRepositoryMock.Reset();
            _accessValidatorMock.Reset();
            _rcImageMock.Reset();
            _rcGetTemplateTagsMock.Reset();
            _rcDepartmentMock.Reset();
            _rcPositionMock.Reset();
            _rcSendEmailMock.Reset();

            RcAddImageSetUp();
            RcSendEmailSetUp();
            RcGetTemplateTagSetUp();
            RcChangePositionSetUp();
            RcChangeDepartmentSetUp();

            _accessValidatorMock
                .Setup(x => x.IsAdmin(null))
                .Returns(true);

            _validatorMock
                .Setup(x => x.Validate(It.IsAny<IValidationContext>()).IsValid)
                .Returns(true);

            _mapperUserMock
                .Setup(x => x.Map(_createUserRequest, _operationResultAddImageMock.Object.Body.Id))
                .Returns(_dbUser);

            _userRepositoryMock
                .Setup(x => x.Create(_dbUser, _createUserRequest.Password))
                .Returns(_dbUser.Id);
        }

        #endregion

        [Test]
        public void ShoulRequestIsPartialSuccessWhenImageWasNotAdded()
        {
            _expectedOperationResultResponse.Status = OperationResultStatusType.PartialSuccess;

            var messageError = new List<string>();
            messageError.Add($"Can not add avatar image to user with id {_dbUser.Id}. Please try again later.");

            _expectedOperationResultResponse.Errors = messageError;

            _operationResultAddImageMock
                .Setup(x => x.IsSuccess)
                .Returns(false);
            _operationResultAddImageMock
                .Setup(x => x.Errors)
                .Returns(messageError);

            _mapperUserMock
                .Setup(x => x.Map(_createUserRequest, null))
                .Returns(_dbUser);

            SerializerAssert.AreEqual(_expectedOperationResultResponse, _command.Execute(_createUserRequest));
            _userRepositoryMock.Verify(x => x.Create(_dbUser, _createUserRequest.Password), Times.Once);
            _rcImageMock.Verify(
                x => x.GetResponse<IOperationResult<IAddImageResponse>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
            _rcSendEmailMock.Verify(
                x => x.GetResponse<IOperationResult<bool>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
            _rcGetTemplateTagsMock.Verify(
               x => x.GetResponse<IOperationResult<IGetEmailTemplateTagsResponse>>(
                   It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
            _rcPositionMock.Verify(
                x => x.GetResponse<IOperationResult<bool>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
            _rcDepartmentMock.Verify(
                x => x.GetResponse<IOperationResult<bool>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
        }

        [Test]
        public void ShouldRequestIsPartialSuccessWhenEmailWasNotSent()
        {
            _expectedOperationResultResponse.Status = OperationResultStatusType.PartialSuccess;

            var messageError = new List<string>();
            messageError.Add($"Can not send email to '{_dbCommunication.Value}'. " +
                "Email placed in resend queue and will be resended in 1 hour.");

            _expectedOperationResultResponse.Errors = messageError;

            _operationResultSendEmailMock
                .Setup(x => x.IsSuccess)
                .Returns(false);
            _operationResultSendEmailMock
                .Setup(x => x.Errors)
                .Returns(messageError);

            SerializerAssert.AreEqual(_expectedOperationResultResponse, _command.Execute(_createUserRequest));
            _userRepositoryMock.Verify(x => x.Create(_dbUser, _createUserRequest.Password), Times.Once);
            _rcImageMock.Verify(
                x => x.GetResponse<IOperationResult<IAddImageResponse>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
            _rcSendEmailMock.Verify(
                x => x.GetResponse<IOperationResult<bool>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
            _rcGetTemplateTagsMock.Verify(
               x => x.GetResponse<IOperationResult<IGetEmailTemplateTagsResponse>>(
                   It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
            _rcPositionMock.Verify(
                x => x.GetResponse<IOperationResult<bool>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
            _rcDepartmentMock.Verify(
                x => x.GetResponse<IOperationResult<bool>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
        }

        [Test]
        public void ShouldRequestIsPartialSuccessWhenDepartmentWasNotChanged()
        {
            _expectedOperationResultResponse.Status = OperationResultStatusType.PartialSuccess;

            var messageError = new List<string>();
            messageError.Add($"Сan't assign user {_dbUser.Id} to the department {_createUserRequest.DepartmentId}. Please try again later.");

            _expectedOperationResultResponse.Errors = messageError;

            _operationResultChangeDepartmentMock
                .Setup(x => x.IsSuccess)
                .Returns(false);
            _operationResultChangeDepartmentMock
                .Setup(x => x.Errors)
                .Returns(messageError);

            SerializerAssert.AreEqual(_expectedOperationResultResponse, _command.Execute(_createUserRequest));
            _userRepositoryMock.Verify(x => x.Create(_dbUser, _createUserRequest.Password), Times.Once);
            _rcImageMock.Verify(
                x => x.GetResponse<IOperationResult<IAddImageResponse>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
            _rcSendEmailMock.Verify(
                x => x.GetResponse<IOperationResult<bool>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
            _rcGetTemplateTagsMock.Verify(
               x => x.GetResponse<IOperationResult<IGetEmailTemplateTagsResponse>>(
                   It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
            _rcPositionMock.Verify(
                x => x.GetResponse<IOperationResult<bool>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
            _rcDepartmentMock.Verify(
                x => x.GetResponse<IOperationResult<bool>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
        }

        [Test]
        public void ShouldRequestIsPartialSuccessWhenPositionWasNotChanged()
        {
            _expectedOperationResultResponse.Status = OperationResultStatusType.PartialSuccess;

            var messageError = new List<string>();
            messageError.Add($"Сan't assign position {_createUserRequest.PositionId} to the user {_dbUser.Id}. Please try again later.");

            _expectedOperationResultResponse.Errors = messageError;

            _operationResultChangePositionMock
                .Setup(x => x.IsSuccess)
                .Returns(false);
            _operationResultChangePositionMock
                .Setup(x => x.Errors)
                .Returns(messageError);

            SerializerAssert.AreEqual(_expectedOperationResultResponse, _command.Execute(_createUserRequest));
            _userRepositoryMock.Verify(x => x.Create(_dbUser, _createUserRequest.Password), Times.Once);
            _rcImageMock.Verify(
                x => x.GetResponse<IOperationResult<IAddImageResponse>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
            _rcSendEmailMock.Verify(
                x => x.GetResponse<IOperationResult<bool>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
            _rcGetTemplateTagsMock.Verify(
               x => x.GetResponse<IOperationResult<IGetEmailTemplateTagsResponse>>(
                   It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
            _rcPositionMock.Verify(
                x => x.GetResponse<IOperationResult<bool>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
            _rcDepartmentMock.Verify(
                x => x.GetResponse<IOperationResult<bool>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
        }

        [Test]
        public void ShouldRequestIsPartialSuccessWhenChangeDepartmentRequestThrowException()
        {
            _expectedOperationResultResponse.Status = OperationResultStatusType.PartialSuccess;

            var messageError = new List<string>();
            messageError.Add($"Сan't assign user {_dbUser.Id} to the department {_createUserRequest.DepartmentId}. Please try again later.");

            _expectedOperationResultResponse.Errors = messageError;

            _rcDepartmentMock.Setup(
                x => x.GetResponse<IOperationResult<bool>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()))
                .Throws(new Exception());

            SerializerAssert.AreEqual(_expectedOperationResultResponse, _command.Execute(_createUserRequest));
            _userRepositoryMock.Verify(x => x.Create(_dbUser, _createUserRequest.Password), Times.Once);
            _rcImageMock.Verify(
                x => x.GetResponse<IOperationResult<IAddImageResponse>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
            _rcSendEmailMock.Verify(
                x => x.GetResponse<IOperationResult<bool>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
            _rcGetTemplateTagsMock.Verify(
               x => x.GetResponse<IOperationResult<IGetEmailTemplateTagsResponse>>(
                   It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
            _rcPositionMock.Verify(
                x => x.GetResponse<IOperationResult<bool>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
            _rcDepartmentMock.Verify(
                x => x.GetResponse<IOperationResult<bool>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
        }

        [Test]
        public void ShouldRequestIsPartialSuccessWhenChangePositionRequestThrowException()
        {
            _expectedOperationResultResponse.Status = OperationResultStatusType.PartialSuccess;

            var messageError = new List<string>();
            messageError.Add($"Сan't assign position {_createUserRequest.PositionId} to the user {_dbUser.Id}. Please try again later.");

            _expectedOperationResultResponse.Errors = messageError;

            _rcPositionMock.Setup(
                x => x.GetResponse<IOperationResult<bool>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()))
                .Throws(new Exception());

            SerializerAssert.AreEqual(_expectedOperationResultResponse, _command.Execute(_createUserRequest));
            _userRepositoryMock.Verify(x => x.Create(_dbUser, _createUserRequest.Password), Times.Once);
            _rcImageMock.Verify(
                x => x.GetResponse<IOperationResult<IAddImageResponse>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
            _rcSendEmailMock.Verify(
                x => x.GetResponse<IOperationResult<bool>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
            _rcGetTemplateTagsMock.Verify(
               x => x.GetResponse<IOperationResult<IGetEmailTemplateTagsResponse>>(
                   It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
            _rcPositionMock.Verify(
                x => x.GetResponse<IOperationResult<bool>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
            _rcDepartmentMock.Verify(
                x => x.GetResponse<IOperationResult<bool>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
        }

        [Test]
        public void ShouldCreateUserSuccessful()
        {
            SerializerAssert.AreEqual(_expectedOperationResultResponse, _command.Execute(_createUserRequest));
            _userRepositoryMock.Verify(x => x.Create(_dbUser, _createUserRequest.Password), Times.Once);
            _rcImageMock.Verify(
                x => x.GetResponse<IOperationResult<IAddImageResponse>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
            _rcSendEmailMock.Verify(
                x => x.GetResponse<IOperationResult<bool>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
            _rcGetTemplateTagsMock.Verify(
               x => x.GetResponse<IOperationResult<IGetEmailTemplateTagsResponse>>(
                   It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
            _rcPositionMock.Verify(
                x => x.GetResponse<IOperationResult<bool>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
            _rcDepartmentMock.Verify(
                x => x.GetResponse<IOperationResult<bool>>(
                    It.IsAny<object>(), default, It.IsAny<RequestTimeout>()), Times.Once);
        }

    }
}
