using LT.DigitalOffice.Broker.Responses;
using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.Kernel.UnitTestLibrary;
using LT.DigitalOffice.UserService.Mappers.Interfaces;
using LT.DigitalOffice.UserService.Mappers.UnitTests.Utils;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers.UnitTests
{
    public class UserMapperTests
    {
        private IMapper<DbUser, User> mapper;
        private IMapper<UserRequest, DbUser> mapperEditUserRequest;

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
            mapper = new UserMapper();
            mapperEditUserRequest = new UserMapper();

            userId = Guid.NewGuid();
            achievementId = Guid.NewGuid();
            certificateFileId = Guid.NewGuid();
            pictureFileId = Guid.NewGuid();
            avatarFileId = Guid.NewGuid();

            achievement = new DbAchievement
            {
                Id = achievementId,
                Message = Message,
                PictureFileId = pictureFileId
            };

            dbUserAchievement = new DbUserAchievement
            {
                Achievement = achievement,
                AchievementId = achievementId,
                User = dbUser,
                Time = DateTime.Now,
                UserId = userId
            };

            dbUserCertificateFile = new DbUserCertificateFile
            {
                CertificateId = certificateFileId,
                User = dbUser,
                UserId = userId
            };

            dbUser = new DbUser
            {
                AchievementsIds = new List<DbUserAchievement> { dbUserAchievement },
                AvatarFileId = avatarFileId,
                FirstName = FirstName,
                Id = userId,
                IsActive = IsActive,
                IsAdmin = IsAdmin,
                LastName = LastName,
                Status = Status,
                CertificatesFilesIds = new List<DbUserCertificateFile> { dbUserCertificateFile }
            };
        }

        #region IMapper<DbUser, User>

        [Test]
        public void ShouldThrowBadRequestExceptionWhenDbUserIsNull()
        {
            Assert.Throws<BadRequestException>(() => mapper.Map(null));
        }

        [Test]
        public void ShouldReturnUserModelWhenMappingValidDbUser()
        {
            var resultUserModel = mapper.Map(dbUser);

            Assert.IsNotNull(resultUserModel);
            Assert.IsInstanceOf<User>(resultUserModel);

            Assert.AreEqual(achievementId, resultUserModel.AchievementsIds.ToList()[0].Id);
            Assert.AreEqual(Message, resultUserModel.AchievementsIds.ToList()[0].Message);
            Assert.AreEqual(pictureFileId, resultUserModel.AchievementsIds.ToList()[0].PictureFileId);

            Assert.AreEqual(certificateFileId, resultUserModel.CertificatesIds.ToList()[0]);
            Assert.AreEqual(userId, resultUserModel.Id);
            Assert.AreEqual(FirstName, resultUserModel.FirstName);
            Assert.AreEqual(LastName, resultUserModel.LastName);
            Assert.IsNull(resultUserModel.MiddleName);
            Assert.AreEqual(Status, resultUserModel.Status);
            Assert.AreEqual(avatarFileId, resultUserModel.AvatarFileId);
            Assert.AreEqual(IsAdmin, resultUserModel.IsAdmin);
        }
        #endregion

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

            var result = mapperEditUserRequest.Map(request);

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

            Assert.Throws<BadRequestException>(() => mapperEditUserRequest.Map(request));
        }
        #endregion
    }
}