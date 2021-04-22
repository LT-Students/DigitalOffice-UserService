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

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _rcSendEmailMock = new Mock<IRequestClient<ISendEmailRequest>>();
            _loggerMock = new Mock<ILogger<EmailResender>>();

            _ = new EmailResender(_rcSendEmailMock.Object, _loggerMock.Object);
            Task.Run(() => EmailResender.Start(0));
        }

        [Test]
        public void ShouldResentMessageWhenEveryoneIsGood()
        {
            var email = new { Id = Guid.NewGuid() };

            var _operationResultMock = new Mock<IOperationResult<bool>>();
            _operationResultMock.Setup(x => x.Body).Returns(true);
            _operationResultMock.Setup(x => x.IsSuccess).Returns(true);
            _operationResultMock.Setup(x => x.Errors).Returns(new List<string>());

            var responseBrokerMock = new Mock<Response<IOperationResult<bool>>>();
            responseBrokerMock
               .SetupGet(x => x.Message)
               .Returns(_operationResultMock.Object);

            _rcSendEmailMock.Setup(
                x => x.GetResponse<IOperationResult<bool>>(
                    email, default, It.IsAny<RequestTimeout>()))
                .Returns(Task.FromResult(responseBrokerMock.Object));

            Console.WriteLine($"The sending broker will successfully send messages.");

            EmailResender.AddToQueue(email);

            Console.WriteLine($"{email} was added to queue.");

            Task.Delay(TimeSpan.FromSeconds(3));

            _rcSendEmailMock.Verify(x => x.GetResponse<IOperationResult<bool>>(
                    email, default, It.IsAny<RequestTimeout>()), Times.Once);
        }

        [Test]
        public void ShouldNotResentMessageWhenClientNotWork()
        {
            var email = new { Id = Guid.NewGuid() };

            var _operationResultMock = new Mock<IOperationResult<bool>>();
            _operationResultMock.Setup(x => x.Body).Returns(false);
            _operationResultMock.Setup(x => x.IsSuccess).Returns(false);
            _operationResultMock.Setup(x => x.Errors).Returns(new List<string>());

            var responseBrokerMock = new Mock<Response<IOperationResult<bool>>>();
            responseBrokerMock
               .SetupGet(x => x.Message)
               .Returns(_operationResultMock.Object);

            _rcSendEmailMock.Setup(
                x => x.GetResponse<IOperationResult<bool>>(
                    email, default, It.IsAny<RequestTimeout>()))
                .Returns(Task.FromResult(responseBrokerMock.Object));

            Console.WriteLine($"The sending broker will failed send messages.");

            EmailResender.AddToQueue(email);

            Console.WriteLine($"{email} was added to queue.");

            Task.Delay(TimeSpan.FromSeconds(3));

            _rcSendEmailMock.Verify(x => x.GetResponse<IOperationResult<bool>>(
                    email, default, It.IsAny<RequestTimeout>()), Times.AtLeastOnce);
        }
    }
}
