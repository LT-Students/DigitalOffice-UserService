using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Mappers.RequestsMappers;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using NUnit.Framework;
using System;

namespace LT.DigitalOffice.UserService.Mappers.UnitTests
{
    public class UserMapperTests
    {
        private UserRequestMapper userRequestMapper;

        private const string Message = "smth";
        private const string FirstName = "Ivan";
        private const string LastName = "Dudikov";
        private const bool IsActive = true;
        private const string Status = "Hello, world!";
        private const bool IsAdmin = false;

        private Guid userId;
        private Guid achievementId;
        private Guid certificateFileId;
        private Guid pictureFileId;
        private Guid avatarFileId;

        private DbAchievement achievement;
        private DbUserAchievement dbUserAchievement;
        private DbUser dbUser;
        private DbUserCertificateFile dbUserCertificateFile;

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
                Status = "Example",
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