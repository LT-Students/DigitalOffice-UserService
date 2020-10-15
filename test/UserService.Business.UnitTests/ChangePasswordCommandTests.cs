using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto;
using Moq;
using NUnit.Framework;

namespace LT.DigitalOffice.UserService.Business.UnitTests
{
    public class ChangePasswordCommandTests
    {
        private Mock<IUserCredentialsRepository> repositoryMock;
        private IChangePasswordCommand command;

        private ChangePasswordRequest request;

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IUserCredentialsRepository>();

            command = new ChangePasswordCommand(repositoryMock.Object);

            request = new ChangePasswordRequest
            {
                Login = "Login",
                NewPassword = "NewPassword"
            };
        }

        [Test]
        public void ShouldThrowBadRequestExceptionWhenRequestFieldsAreNull()
        {
            repositoryMock
                .Setup(x => x.ChangePassword(It.IsAny<string>(), It.IsAny<string>()))
                .Throws<BadRequestException>();

            Assert.Throws<BadRequestException>(() => command.Execute(request));
        }

        [Test]
        public void ShouldThrowNotFoundExceptionWhenLoginWasNotFound()
        {
            repositoryMock
                .Setup(x => x.ChangePassword(It.IsAny<string>(), It.IsAny<string>()))
                .Throws<NotFoundException>();

            Assert.Throws<NotFoundException>(() => command.Execute(request));
        }

        [Test]
        public void ShouldChangePassword()
        {
            repositoryMock
                .Setup(x => x.ChangePassword(It.IsAny<string>(), It.IsAny<string>()));

            Assert.DoesNotThrow(() => command.Execute(request));
            repositoryMock.Verify(repository => repository.ChangePassword(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
