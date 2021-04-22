using LT.DigitalOffice.Broker.Requests;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.UserService.Business.Helpers.Email;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.UnitTests
{
    public class EmailResenderTests
    {
        private static Mock<IRequestClient<ISendEmailRequest>> _rcSendEmailMock;
        private static Mock<ILogger<EmailResender>> _loggerMock;

        private static EmailResender _emailResender;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _rcSendEmailMock = new Mock<IRequestClient<ISendEmailRequest>>();
            _loggerMock = new Mock<ILogger<EmailResender>>();

            _emailResender = new EmailResender(_rcSendEmailMock.Object, _loggerMock.Object);
            Task.Run(() => EmailResender.Start(0));
        }

        [Test]
        public void ShouldCorrectlySendMessage()
        {
            var email = new { Id = Guid.NewGuid() };

            var _operationResultAddImageMock = new Mock<IOperationResult<bool>>();
            _operationResultAddImageMock.Setup(x => x.Body).Returns(true);
            _operationResultAddImageMock.Setup(x => x.IsSuccess).Returns(true);
            _operationResultAddImageMock.Setup(x => x.Errors).Returns(new List<string>());

            var responseBrokerAddImageMock = new Mock<Response<IOperationResult<bool>>>();
            responseBrokerAddImageMock
               .SetupGet(x => x.Message)
               .Returns(_operationResultAddImageMock.Object);

            _rcSendEmailMock.Setup(
                x => x.GetResponse<IOperationResult<bool>>(
                    email, default, It.IsAny<RequestTimeout>()))
                .Returns(Task.FromResult(responseBrokerAddImageMock.Object));

            EmailResender.AddToQueue(email);

            Task.Delay(TimeSpan.FromSeconds(3));

            _rcSendEmailMock.Verify(x => x.GetResponse<IOperationResult<bool>>(
                    email, default, It.IsAny<RequestTimeout>()), Times.Once);
        }

        [Test]
        public void ShouldIncorrectlySendMessage()
        {
            var email = new { Id = Guid.NewGuid() };

            var _operationResultAddImageMock = new Mock<IOperationResult<bool>>();
            _operationResultAddImageMock.Setup(x => x.Body).Returns(false);
            _operationResultAddImageMock.Setup(x => x.IsSuccess).Returns(false);
            _operationResultAddImageMock.Setup(x => x.Errors).Returns(new List<string>());

            var responseBrokerAddImageMock = new Mock<Response<IOperationResult<bool>>>();
            responseBrokerAddImageMock
               .SetupGet(x => x.Message)
               .Returns(_operationResultAddImageMock.Object);

            _rcSendEmailMock.Setup(
                x => x.GetResponse<IOperationResult<bool>>(
                    email, default, It.IsAny<RequestTimeout>()))
                .Returns(Task.FromResult(responseBrokerAddImageMock.Object));

            EmailResender.AddToQueue(email);

            Task.Delay(TimeSpan.FromSeconds(3));

            _rcSendEmailMock.Verify(x => x.GetResponse<IOperationResult<bool>>(
                    email, default, It.IsAny<RequestTimeout>()), Times.AtLeastOnce);
        }
    }
}
