using LT.DigitalOffice.Kernel.Exceptions;
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
    public class GetAllUsersCommandTests
    {
        private Mock<IUserRepository> repositoryMock;
        private Mock<IMapper<DbUser, User>> mapperMock;
        private IGetAllUsersCommand command;

        private Guid userId;
        private User user;
        private DbUser dbUser;
        private int skipCount;
        private int takeCount;
        private string nameFilter;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            repositoryMock = new Mock<IUserRepository>();
            mapperMock = new Mock<IMapper<DbUser, User>>();

            command = new GetAllUsersCommand(repositoryMock.Object, mapperMock.Object);

            userId = Guid.NewGuid();
            user = new User { Id = userId };
            dbUser = new DbUser { Id = userId };

            skipCount = 0;
            takeCount = 1;
            nameFilter = "";
        }

        [Test]
        public void ShouldThrowExceptionWhenRepositoryThrowsIt()
        {
            repositoryMock
                .Setup(x => x.GetAllUsers(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .Throws(new NotFoundException());

            Assert.Throws<NotFoundException>(() => command.Execute(skipCount, takeCount, nameFilter));
        }

        [Test]
        public void ShouldThrowExceptionWhenMapperThrowsIt()
        {
            repositoryMock
                .Setup(x => x.GetAllUsers(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(new List<DbUser>() { dbUser });

            mapperMock
                .Setup(x => x.Map(dbUser))
                .Throws(new BadRequestException());

            Assert.Throws<BadRequestException>(() => command.Execute(skipCount, takeCount, nameFilter));
        }

        [Test]
        public void ShouldReturnUsers()
        {
            repositoryMock
                .Setup(x => x.GetAllUsers(skipCount, takeCount, nameFilter))
                .Returns(new List<DbUser>() { dbUser });

            mapperMock
                .Setup(x => x.Map(It.IsAny<DbUser>()))
                .Returns(user);

            Assert.DoesNotThrow(() => command.Execute(skipCount, takeCount, nameFilter));
        }
    }
}
