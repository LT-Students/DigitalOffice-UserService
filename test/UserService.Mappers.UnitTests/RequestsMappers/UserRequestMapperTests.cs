using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using NUnit.Framework;
using System;

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
                Status = UserStatus.Sick,
                Password = "Example",
                IsAdmin = false,
                IsActive = true,
                AvatarFileId = Guid.NewGuid()
            };

            var result = userRequestMapper.Map(request);

            var user = new DbUser()
            {
                Id = (Guid)request.Id,
                Email = "Example@gmail.com",
                FirstName = "Example",
                LastName = "Example",
                MiddleName = "Example",
                Status = 1,
                IsAdmin = false,
                IsActive = true,
                AvatarFileId = request.AvatarFileId
            };

            SerializerAssert.AreEqual(user, result);
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