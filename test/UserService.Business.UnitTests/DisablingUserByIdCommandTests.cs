using LT.DigitalOffice.Kernel.AccessValidator.Interfaces;
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
        private Guid requestingUserId;
        #endregion

        #region Setup
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            accessValidatorMock = new Mock<IAccessValidator>();
            userId = Guid.NewGuid();
            requestingUserId = Guid.NewGuid();

            newDbUser = new DbUser
            {
                Id = userId,
                FirstName = "Example1",
                LastName = "Example1",
                MiddleName = "Example1",
                Status = "normal",
                AvatarFileId = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = false
            };
        }

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IUserRepository>();
            command = new DisableUserByIdCommand(repositoryMock.Object, accessValidatorMock.Object);
        }
        #endregion

        [Test]
        public void ShouldThrowExceptionWhenUserIdNotFoundInDb()
        {
            accessValidatorMock
                .Setup(x => x.IsAdmin())
                .Returns(true);

            accessValidatorMock
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(true);

            repositoryMock
                .Setup(x => x.GetUserInfoById(userId))
                .Throws(new Exception());

            Assert.Throws<Exception>(() => command.Execute(userId, requestingUserId));
            repositoryMock.Verify(repository => repository.EditUser(It.IsAny<DbUser>()), Times.Never);
        }

        [Test]
        public void ShouldThrowExceptionWhenEditingUserInDb()
        {
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
                .Throws(new Exception());

            Assert.Throws<Exception>(() => command.Execute(userId, requestingUserId));
        }

        [Test]
        public void ShouldUserDisabledSuccess()
        {
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

            Assert.DoesNotThrow(() => command.Execute(userId, requestingUserId));
        }

        [Test]
        public void ShouldUserDisabledSuccessWhenUserIsAdmin()
        {
            accessValidatorMock
                .Setup(x => x.IsAdmin())
                .Returns(true);

            repositoryMock
                .Setup(x => x.GetUserInfoById(userId))
                .Returns(newDbUser);

            repositoryMock
                .Setup(x => x.EditUser(It.IsAny<DbUser>()))
                .Returns(true);

            Assert.DoesNotThrow(() => command.Execute(userId, requestingUserId));
        }

        [Test]
        public void ShouldUserDisabledSuccessWhenUserIsNotAdminAndHasRights()
        {
            accessValidatorMock
                .Setup(x => x.IsAdmin())
                .Returns(false);

            accessValidatorMock
                .Setup(x => x.HasRights(It.IsAny<int>()))
                .Returns(true);

            repositoryMock
                .Setup(x => x.GetUserInfoById(userId))
                .Returns(newDbUser);

            repositoryMock
                .Setup(x => x.EditUser(It.IsAny<DbUser>()))
                .Returns(true);

            Assert.DoesNotThrow(() => command.Execute(userId, requestingUserId));
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

            Assert.That(() => command.Execute(userId, requestingUserId),
                Throws.InstanceOf<Exception>().And.Message.EqualTo("Not enough rights."));
        }
    }
}
