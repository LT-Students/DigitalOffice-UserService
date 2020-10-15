using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.Kernel.UnitTestLibrary;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Business.UnitTests
{
    public class GetUsersByIdsCommandTests
    {
        private IGetUsersByIdsCommand command;
        private Mock<IUserRepository> repositoryMock;
        private Mock<IMapper<DbUser, User>> mapperMock;

        private Guid userId;
        private List<Guid> userIdList;
        private User user;
        private List<User> usersList;
        private DbUser dbUser;

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IUserRepository>();
            mapperMock = new Mock<IMapper<DbUser, User>>();
            command = new GetUsersByIdsCommand(repositoryMock.Object, mapperMock.Object);

            userId = Guid.NewGuid();
            userIdList = new List<Guid>() { userId };
            user = new User { Id = userId };
            usersList = new List<User>() { user };
            dbUser = new DbUser { Id = userId };
        }

        [Test]
        public void ShouldReturnModelOfUser()
        {
            repositoryMock.Setup(repository => repository.GetUserInfoById(userId)).Returns(dbUser).Verifiable();
            mapperMock.Setup(mapper => mapper.Map(dbUser)).Returns(user).Verifiable();

            SerializerAssert.AreEqual(usersList, command.Execute(userIdList));
            repositoryMock.Verify();
            mapperMock.Verify();
        }

        [Test]
        public void ShouldThrowExceptionWhenMapperThrowsException()
        {
            repositoryMock.Setup(repository => repository.GetUserInfoById(It.IsAny<Guid>())).Returns(dbUser).Verifiable();
            mapperMock.Setup(mapper => mapper.Map(It.IsAny<DbUser>())).Throws<BadRequestException>().Verifiable();

            Assert.Throws<BadRequestException>(() => command.Execute(userIdList));
            mapperMock.Verify();
            repositoryMock.Verify();
        }

        [Test]
        public void ShouldThrowExceptionWhenRepositoryThrowsException()
        {
            repositoryMock.Setup(repository => repository.GetUserInfoById(userId)).Throws<NotFoundException>().Verifiable();
            mapperMock.Setup(mapper => mapper.Map(dbUser)).Returns(user);

            Assert.Throws<NotFoundException>(() => command.Execute(userIdList));
            repositoryMock.Verify();
            mapperMock.Verify(mapper => mapper.Map(It.IsAny<DbUser>()), Times.Never);
        }
    }
}