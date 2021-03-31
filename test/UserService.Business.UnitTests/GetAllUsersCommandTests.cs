﻿using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.ResponsesMappers.Interfaces;
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
        private Mock<IUserResponseMapper> mapperMock;
        private IGetAllUsersCommand command;

        private Guid userId;
        private User user;
        private DbUser dbUser;
        private int skipCount;
        private int takeCount;
        private string userNameFilter;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            repositoryMock = new Mock<IUserRepository>();
            mapperMock = new Mock<IUserResponseMapper>();

            command = new GetAllUsersCommand(repositoryMock.Object, mapperMock.Object);

            userId = Guid.NewGuid();
            user = new User { Id = userId };
            dbUser = new DbUser { Id = userId };

            skipCount = 0;
            takeCount = 1;
            userNameFilter = "";
        }

        [Test]
        public void ShouldThrowExceptionWhenRepositoryThrowsIt()
        {
            repositoryMock
                .Setup(x => x.GetAllUsers(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .Throws(new NotFoundException());

            Assert.Throws<NotFoundException>(() => command.Execute(skipCount, takeCount, userNameFilter));
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

            Assert.Throws<BadRequestException>(() => command.Execute(skipCount, takeCount, userNameFilter));
        }

        [Test]
        public void ShouldReturnUsers()
        {
            repositoryMock
                .Setup(x => x.GetAllUsers(skipCount, takeCount, userNameFilter))
                .Returns(new List<DbUser>() { dbUser });

            mapperMock
                .Setup(x => x.Map(It.IsAny<DbUser>()))
                .Returns(user);

            Assert.DoesNotThrow(() => command.Execute(skipCount, takeCount, userNameFilter));
        }
    }
}
