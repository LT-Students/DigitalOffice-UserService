using LT.DigitalOffice.Broker.Responses;
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
        private IMapper<EditUserRequest, DbUser> mapperEditUserRequest;
        private IMapper<DbUser, string, object> mapper2;

        private const string Email = "smth@emal.com";
        private const string Message = "smth";
        private const string FirstName = "Ivan";
        private const string MiddleName = "Ivanovich";
        private const string LastName = "Dudikov";
        private const string PasswordHash = "42";
        private const bool IsActive = true;
        private const string Status = "Hello, world!";
        private const bool IsAdmin = false;
        private const string UserPositionName = "Software Engineer";

        private Guid userId;
        private Guid achievementId;
        private Guid certificateFileId;
        private Guid pictureFileId;
        private Guid avatarFileId;

        private DbAchievement achievement;
        private DbUserAchievement dbUserAchievement;
        private DbUser dbUser;
        private DbUserCertificateFile dbUserCertificateFile;
        private IUserPositionResponse userPosition;

        [SetUp]
        public void SetUp()
        {
            mapper = new UserMapper();
            mapperEditUserRequest = new UserMapper();
            mapper2 = new UserMapper();

            userId = Guid.NewGuid();
            achievementId = Guid.NewGuid();
            certificateFileId = Guid.NewGuid();
            pictureFileId = Guid.NewGuid();
            avatarFileId = Guid.NewGuid();
            achievement = new DbAchievement {Id = achievementId, Message = Message, PictureFileId = pictureFileId};
            dbUserAchievement = new DbUserAchievement
            {
                Achievement = achievement, AchievementId = achievementId, User = dbUser, Time = DateTime.Now,
                UserId = userId
            };
            dbUserCertificateFile = new DbUserCertificateFile
                {CertificateId = certificateFileId, User = dbUser, UserId = userId};
            dbUser = new DbUser
            {
                AchievementsIds = new List<DbUserAchievement> {dbUserAchievement}, AvatarFileId = avatarFileId,
                FirstName = FirstName, Id = userId, IsActive = IsActive, IsAdmin = IsAdmin, LastName = LastName,
            };
        }

        #region IMapper<DbUser, User>

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenDbUserIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => mapper.Map(null));
        }

        [Test]
        public void ShouldReturnUserModelWhenMappingValidDbUser()
        {
            var resultUserModel = mapper.Map(dbUser);

            Assert.IsNotNull(resultUserModel);
            Assert.IsInstanceOf<User>(resultUserModel);

            Assert.AreEqual(achievementId, resultUserModel.Achievements.ToList()[0].Id);
            Assert.AreEqual(Message, resultUserModel.Achievements.ToList()[0].Message);
            Assert.AreEqual(pictureFileId, resultUserModel.Achievements.ToList()[0].PictureFileId);

            Assert.AreEqual(certificateFileId, resultUserModel.CertificatesIds.ToList()[0]);
            Assert.AreEqual(userId, resultUserModel.Id);
            Assert.AreEqual(FirstName, resultUserModel.FirstName);
            Assert.AreEqual(LastName, resultUserModel.LastName);
            Assert.IsNull(resultUserModel.MiddleName);
            Assert.AreEqual(Email, resultUserModel.Email);
            Assert.AreEqual(Status, resultUserModel.Status);
            Assert.AreEqual(avatarFileId, resultUserModel.AvatarId);
            Assert.AreEqual(IsAdmin, resultUserModel.IsAdmin);
        }
        #endregion

        #region EditUserRequest to DbUser
        [Test]
        public void ShouldReturnNewDbUserWhenDataCorrect()
        {
            var request = new EditUserRequest()
            {
                Id = Guid.NewGuid(),
                Email = "Example@gmail.com",
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
                Id = request.Id,
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
            var request = new EditUserRequest();
            Assert.Throws<ArgumentNullException>(() => mapperEditUserRequest.Map(request));
        }
        #endregion

        #region ITwoModelsMapper<DbUser, IUserPositionResponse, object>
        [Test]
        public void ShouldThrowArgumentNullExceptionWhenFirstArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => mapper2.Map(dbUser, null));
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenSecondArgumentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => mapper2.Map(dbUser, null));
        }

        [Test]
        public void ShouldReturnModel()
        {
            userPosition = new InheritedUserPositionResponse
            {
                UserPositionName = UserPositionName
            };

            dbUser.MiddleName = MiddleName;

            var expectedResult = new
            {
                UserId = userId,
                FirstName = FirstName,
                LastName = LastName,
                MiddleName = MiddleName,
                UserPosition = userPosition.UserPositionName
            };

            var result = mapper2.Map(dbUser, userPosition.UserPositionName);

            SerializerAssert.AreEqual(expectedResult, result);
        }
        #endregion
    }
}