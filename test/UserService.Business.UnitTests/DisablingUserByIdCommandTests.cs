using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using Moq;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.UserService.Business.UnitTests
{
    public class DisablingUserByIdCommandTests
    {
        #region Variables declaration
        private Mock<IUserRepository> repositoryMock;
        private Mock<IAccessValidator> accessValidatorMock;

        private IDisableUserByIdCommand command;

        private Guid userId;
        private DbUser newDbUser;
        #endregion

        #region Setup
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            userId = Guid.NewGuid();

            newDbUser = new DbUser
            {
                Id = userId,
                FirstName = "Example1",
                LastName = "Example1",
                MiddleName = "Example1",
                Status = 1,
                AvatarFileId = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = false
            };
        }

        [SetUp]
        public void SetUp()
        {
            accessValidatorMock = new Mock<IAccessValidator>();
            repositoryMock = new Mock<IUserRepository>();
            command = new DisableUserByIdCommand(repositoryMock.Object, accessValidatorMock.Object);

            accessValidatorMock
                .Setup(x => x.IsAdmin())
                .Returns(true);

            accessValidatorMock
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(true);

            repositoryMock
                .Setup(x => x.GetUserInfoById(userId))
                .Returns(newDbUser);

            repositoryMock
                .Setup(x => x.EditUser(It.IsAny<DbUser>()))
                .Returns(true);
        }
        #endregion

        [Test]
        public void ShouldThrowExceptionWhenUserIdNotFoundInDb()
        {
            repositoryMock
                .Setup(x => x.GetUserInfoById(userId))
                .Throws(new Exception());

            Assert.Throws<Exception>(() => command.Execute(userId));
            repositoryMock.Verify(repository => repository.GetUserInfoById(userId), Times.Once);
            repositoryMock.Verify(repository => repository.EditUser(It.IsAny<DbUser>()), Times.Never);
        }

        [Test]
        public void ShouldThrowExceptionWhenEditingUserInDb()
        {
            repositoryMock
                .Setup(x => x.EditUser(It.IsAny<DbUser>()))
                .Throws(new Exception());

            Assert.Throws<Exception>(() => command.Execute(userId));
            repositoryMock.Verify(repository => repository.GetUserInfoById(userId), Times.Once);
            repositoryMock.Verify(repository => repository.EditUser(It.IsAny<DbUser>()), Times.Once);
        }

        [Test]
        public void ShouldUserDisabledSuccess()
        {
            Assert.DoesNotThrow(() => command.Execute(userId));

            repositoryMock.Verify(repository => repository.GetUserInfoById(userId), Times.Once);
            repositoryMock.Verify(repository => repository.EditUser(It.IsAny<DbUser>()), Times.Once);
        }

        [Test]
        public void ShouldUserDisabledSuccessWhenUserIsAdmin()
        {
            accessValidatorMock
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(false);

            command.Execute(userId);

            repositoryMock.Verify(repository => repository.GetUserInfoById(userId), Times.Once);
            repositoryMock.Verify(repository => repository.EditUser(It.IsAny<DbUser>()), Times.Once);
            Assert.DoesNotThrow(() => command.Execute(userId));
        }

        [Test]
        public void ShouldUserDisabledSuccessWhenUserIsNotAdminAndHasRights()
        {
            accessValidatorMock
                .Setup(x => x.IsAdmin())
                .Returns(false);

            Assert.DoesNotThrow(() => command.Execute(userId));
            repositoryMock.Verify(repository => repository.GetUserInfoById(userId), Times.Once);
            repositoryMock.Verify(repository => repository.EditUser(It.IsAny<DbUser>()), Times.Once);
        }

        [Test]
        public void ShouldThrowExceptionWhenUserHasNotEnoughRights()
        {
            accessValidatorMock
                .Setup(x => x.IsAdmin())
                .Returns(false);

            accessValidatorMock
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(false);

            Assert.That(() => command.Execute(userId),
                Throws.InstanceOf<Exception>().And.Message.EqualTo("Not enough rights."));
            repositoryMock.Verify(repository => repository.GetUserInfoById(userId), Times.Never);
            repositoryMock.Verify(repository => repository.EditUser(It.IsAny<DbUser>()), Times.Never);
        }
    }
}
