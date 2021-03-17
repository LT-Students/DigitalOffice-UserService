using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers.ResponsesMappers.UnitTests
{
    internal class UserResponseMapperTests
    {
        private UserResponseMapper userResponseMapper;

        private const string Message = "smth";
        private const string FirstName = "Ivan";
        private const string LastName = "Dudikov";
        private const bool IsActive = true;
        private const int dbStatus = 1;
        private const UserStatus status = UserStatus.Sick;
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
            userResponseMapper = new UserResponseMapper();

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
                Status = dbStatus,
                CertificatesFilesIds = new List<DbUserCertificateFile> { dbUserCertificateFile }
            };
        }

        [Test]
        public void ShouldThrowBadRequestExceptionWhenDbUserIsNull()
        {
            Assert.Throws<BadRequestException>(() => userResponseMapper.Map(null));
        }

        [Test]
        public void ShouldReturnUserModelWhenMappingValidDbUser()
        {
            var resultUserModel = userResponseMapper.Map(dbUser);

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
            Assert.AreEqual(status, resultUserModel.Status);
            Assert.AreEqual(avatarFileId, resultUserModel.AvatarFileId);
            Assert.AreEqual(IsAdmin, resultUserModel.IsAdmin);
        }
    }
}
