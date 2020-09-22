using LT.DigitalOffice.Kernel.UnitTestLibrary;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using Moq;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.UserService.Business.UnitTests
{
    public class GetUserByIdCommandTests
    {
        private IGetUserByIdCommand getUserInfoByIdCommand;
        private Mock<IUserRepository> repositoryMock;
        private Mock<IMapper<DbUser, User>> mapperMock;

        private Guid userId;
        private User user;
        private DbUser dbUser;

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IUserRepository>();
            mapperMock = new Mock<IMapper<DbUser, User>>();
            getUserInfoByIdCommand = new GetUserByIdCommand(repositoryMock.Object, mapperMock.Object);

            userId = Guid.NewGuid();
            user = new User { Id = userId };
            dbUser = new DbUser { Id = userId };
        }

        [Test]
        public void ShouldReturnModelOfUser()
        {
            repositoryMock.Setup(repository => repository.GetUserInfoById(userId)).Returns(dbUser).Verifiable();
            mapperMock.Setup(mapper => mapper.Map(dbUser)).Returns(user).Verifiable();

            SerializerAssert.AreEqual(user, getUserInfoByIdCommand.Execute(userId));
            repositoryMock.Verify();
            mapperMock.Verify();
        }

        [Test]
        public void ShouldThrowExceptionWhenMapperThrowsException()
        {
            repositoryMock.Setup(repository => repository.GetUserInfoById(It.IsAny<Guid>())).Returns(dbUser).Verifiable();
            mapperMock.Setup(mapper => mapper.Map(It.IsAny<DbUser>())).Throws<Exception>().Verifiable();

            Assert.Throws<Exception>(() => getUserInfoByIdCommand.Execute(It.IsAny<Guid>()));
            mapperMock.Verify();
            repositoryMock.Verify();
        }

        [Test]
        public void ShouldThrowExceptionWhenRepositoryThrowsException()
        {
            repositoryMock.Setup(repository => repository.GetUserInfoById(userId)).Throws<Exception>().Verifiable();
            mapperMock.Setup(mapper => mapper.Map(dbUser)).Returns(user);

            Assert.Throws<Exception>(() => getUserInfoByIdCommand.Execute(userId));
            repositoryMock.Verify();
            mapperMock.Verify(mapper => mapper.Map(It.IsAny<DbUser>()), Times.Never);
        }
    }
}