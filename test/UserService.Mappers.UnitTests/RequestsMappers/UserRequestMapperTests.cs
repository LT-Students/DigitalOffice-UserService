using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers.RequestsMappers.UnitTests
{
    internal class UserRequestMapperTests
    {
        private UserRequestMapper userRequestMapper;

        [SetUp]
        public void SetUp()
        {
            userRequestMapper = new UserRequestMapper();
        }

        #region EditUserRequest to DbUser
        [Test]
        public void ShouldReturnNewDbUserWhenDataCorrect()
        {
            var request = new UserRequest()
            {
                Id = Guid.NewGuid(),
                Email = "Example@gmail.com",
                Login = "Example",
                FirstName = "Example",
                LastName = "Example",
                MiddleName = "Example",
                Status = "Example",
                Password = "Example",
                IsAdmin = false,
                IsActive = true,
                AvatarFileId = Guid.NewGuid(),
                Connections = new List<UserConnection>() 
                { 
                    new UserConnection()
                    { 
                        Type = ConnectionType.Email,
                        Value = "Ex@mail.ru"
                    }
                }
            };

            var result = userRequestMapper.Map(request);
            var connectionId = result.Connections.FirstOrDefault().Id;
            var user = new DbUser()
            {
                Id = (Guid)request.Id,
                Email = "Example@gmail.com",
                FirstName = "Example",
                LastName = "Example",
                MiddleName = "Example",
                Status = "Example",
                IsAdmin = false,
                IsActive = true,
                AvatarFileId = request.AvatarFileId,
                Connections = new List<DbConnection>
                {
                    new DbConnection()
                    {
                        Id = connectionId,
                        Type = (int)ConnectionType.Email,
                        Value = "Ex@mail.ru",
                        UserId = (Guid)request.Id
                    }
                }
            };
            SerializerAssert.AreEqual(user, result);
        }

        [Test]
        public void ShouldReturnNewDbUserWhenDataWithEmptyConnections()
        {
            var request = new UserRequest()
            {
                Id = Guid.NewGuid(),
                Email = "Example@gmail.com",
                Login = "Example",
                FirstName = "Example",
                LastName = "Example",
                MiddleName = "Example",
                Status = "Example",
                Password = "Example",
                IsAdmin = false,
                IsActive = true,
                AvatarFileId = Guid.NewGuid(),
                Connections = null
            };

            var user = new DbUser()
            {
                Id = (Guid)request.Id,
                Email = "Example@gmail.com",
                FirstName = "Example",
                LastName = "Example",
                MiddleName = "Example",
                Status = "Example",
                IsAdmin = false,
                IsActive = true,
                AvatarFileId = request.AvatarFileId,
                Connections = null
            };
            SerializerAssert.AreEqual(user, request);
        }

        [Test]
        public void ShouldThrowExceptionWhenRequestIsNull()
        {
            UserRequest request = null;

            Assert.Throws<BadRequestException>(() => userRequestMapper.Map(request));
        }
        #endregion
    }
}