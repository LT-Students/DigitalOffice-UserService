using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.UserService.Business.UnitTests
{
    internal class ChangePasswordCommandTests
    {
        private Mock<IUserCredentialsRepository> repositoryMock;
        private IChangePasswordCommand command;

        private IMemoryCache cache;
        private ChangePasswordRequest changePasswordRequest;

        private Guid generatedId;
        private Guid userId;

        [SetUp]
        public void SetUp()
        {
            generatedId = Guid.NewGuid();
            userId = Guid.NewGuid();

            repositoryMock = new Mock<IUserCredentialsRepository>();

            cache = new MemoryCache(new MemoryCacheOptions());

            command = new ChangePasswordCommand(cache, repositoryMock.Object);

            changePasswordRequest = new ChangePasswordRequest
            {
                GeneratedId = generatedId,
                UserId = userId,
                Login = "Login",
                NewPassword = "NewPassword"
            };

            cache.Set(generatedId, userId, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });
        }

        [Test]
        public void ShouldThrowForbiddenExceptionWhenCacheNotContainKey()
        {
            ChangePasswordRequest newRequest = new ChangePasswordRequest
            {
                GeneratedId = Guid.NewGuid(),
                Login = "Login",
                NewPassword = "NewPassword"
            };

            Assert.Throws<ForbiddenException>(() => command.Execute(newRequest));
        }

        [Test]
        public void ShouldThrowForbiddenExceptionWhenUserIdIsWrong()
        {
            ChangePasswordRequest newRequest = new ChangePasswordRequest
            {
                GeneratedId = changePasswordRequest.GeneratedId,
                UserId = Guid.NewGuid(),
                Login = "Login",
                NewPassword = "NewPassword"
            };

            Assert.Throws<ForbiddenException>(() => command.Execute(newRequest));
        }

        [Test]
        public void ShouldThrowBadRequestExceptionWhenLoginIsEmpty()
        {
            ChangePasswordRequest newRequest = new ChangePasswordRequest
            {
                Login = "",
                NewPassword = "NewPassword"
            };

            Assert.Throws<BadRequestException>(() => command.Execute(newRequest));
        }

        [Test]
        public void ShouldThrowBadRequestExceptionWhenNewPasswordIsEmpty()
        {
            ChangePasswordRequest newRequest = new ChangePasswordRequest
            {
                Login = "Login",
                NewPassword = ""
            };

            Assert.Throws<BadRequestException>(() => command.Execute(newRequest));
        }

        [Test]
        public void ShouldThrowNotFoundExceptionWhenLoginWasNotFound()
        {
            repositoryMock
                .Setup(x => x.ChangePassword(It.IsAny<string>(), It.IsAny<string>()))
                .Throws<NotFoundException>();

            Assert.Throws<NotFoundException>(() => command.Execute(changePasswordRequest));
        }

        [Test]
        public void ShouldChangePassword()
        {
            repositoryMock
                .Setup(x => x.ChangePassword(It.IsAny<string>(), It.IsAny<string>()));

            Assert.DoesNotThrow(() => command.Execute(changePasswordRequest));
            repositoryMock.Verify(repository => repository.ChangePassword(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
