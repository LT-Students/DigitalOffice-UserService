using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Responses;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters;
using LT.DigitalOffice.UserService.Models.Dto.Responses.User;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Mappers.ResponsesMappers.UnitTests
{
    class UserResponseMapperTests
    {
        private UserResponseMapper _mapper;
        private AutoMocker _mocker;

        private DbUser _dbUser;
        private DepartmentInfo _departmentInfo;
        private PositionInfo _positionInfo;
        private List<ProjectInfo> _projects;
        private List<ImageInfo> _images;
        private GetUserFilter _filter;
        private List<string> _errors;

        private UserInfo _userInfo;

        private DbUserAchievement _dbUserAchievement;
        private ImageInfo _imageAchievement;
        private UserAchievementInfo _achievementInfo;

        private DbUserCertificate _dbUserCertificate;
        private ImageInfo _imageCertificate;
        private CertificateInfo _certificateInfo;

        private DbUserEducation _dbUserEducation;
        private EducationInfo _educationInfo;

        private ImageInfo _avatarInfo;
        private CommunicationInfo _communicationInfo;

        private DbSkill _dbSkill;
        private DbUserCommunication _dbUserCommunication;

        private UserResponse _response;

        private void CreateModels()
        {
            #region certificate models
            _dbUserCertificate = new DbUserCertificate
            {
                Id = Guid.NewGuid(),
                Name = "Certificate name",
                SchoolName = "School name",
                EducationType = 0,
                ReceivedAt = DateTime.UtcNow,
                ImageId = Guid.NewGuid()
            };

            _imageCertificate = new ImageInfo
            {
                Id = _dbUserCertificate.ImageId,
                Content = "Content",
                Extension = ".jpg",
                ParentId = null
            };

            _certificateInfo = new CertificateInfo
            {
                Id = _dbUserCertificate.Id,
                Name = _dbUserCertificate.Name,
                SchoolName = _dbUserCertificate.SchoolName,
                EducationType = (EducationType)_dbUserCertificate.EducationType,
                ReceivedAt = _dbUserCertificate.ReceivedAt,
                Image = _imageCertificate
            };
            #endregion

            #region achivment models

            _imageAchievement = new ImageInfo
            {
                Id = Guid.NewGuid(),
                Content = "Content",
                Extension = ".png",
                ParentId = null
            };

            _dbUserAchievement = new DbUserAchievement
            {
                Id = Guid.NewGuid(),
                ReceivedAt = DateTime.UtcNow,
                Achievement = new DbAchievement
                {
                    Id = Guid.NewGuid(),
                    Name = "Achievment name",
                    Description = "Description",
                    ImageId = _imageAchievement.Id.Value
                }
            };

            _achievementInfo = new UserAchievementInfo
            {
                Id = _dbUserAchievement.Id,
                AchievementId = _dbUserAchievement.Achievement.Id,
                ReceivedAt = _dbUserAchievement.ReceivedAt,
                Image = _imageAchievement,
                Name = _dbUserAchievement.Achievement.Name
            };

            #endregion

            #region communication models

            _dbUserCommunication = new DbUserCommunication
            {
                Id = Guid.NewGuid(),
                Value = "value",
                Type = 0
            };

            _communicationInfo = new CommunicationInfo
            {
                Id = _dbUserCommunication.Id,
                Type = (CommunicationType)_dbUserCommunication.Type,
                Value = _dbUserCommunication.Value
            };

            #endregion

            #region education models

            _dbUserEducation = new DbUserEducation
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                UniversityName = "university name",
                QualificationName = "qualification name",
                FormEducation = 0,
                AdmissionAt = DateTime.UtcNow,
                IssueAt = DateTime.UtcNow
            };

            _educationInfo = new EducationInfo
            {
                Id = _dbUserEducation.Id,
                UniversityName = _dbUserEducation.UniversityName,
                QualificationName = _dbUserEducation.QualificationName,
                FormEducation = (FormEducation)_dbUserEducation.FormEducation,
                AdmissionAt = _dbUserEducation.AdmissionAt,
                IssueAt = _dbUserEducation.IssueAt
            };

            #endregion

            _dbSkill = new DbSkill
            {
                Id = Guid.NewGuid(),
                Name = "Skill name"
            };

            _dbUser = new DbUser
            {
                Id = Guid.NewGuid(),
                FirstName = "Name",
                LastName = "LastName",
                DateOfBirth = DateTime.Parse("2021-01-01"),
                Gender = 0,
                City = "Spb",
                Status = 0,
                AvatarFileId = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = false,
                Rate = 1,
                StartWorkingAt = DateTime.UtcNow,
                Skills = new List<DbUserSkill>
                {
                    new DbUserSkill
                    {
                        Skill = _dbSkill
                    }
                },
                Certificates = new List<DbUserCertificate>
                {
                    _dbUserCertificate
                },
                Achievements = new List<DbUserAchievement>
                {
                    _dbUserAchievement
                },
                Communications = new List<DbUserCommunication>
                {
                    _dbUserCommunication
                },
                Educations = new List<DbUserEducation>
                {
                    _dbUserEducation
                }
            };

            _userInfo = new UserInfo
            {
                Id = _dbUser.Id,
                FirstName = _dbUser.FirstName,
                MiddleName = _dbUser.MiddleName,
                DateOfBirth = _dbUser.DateOfBirth.ToString(),
                Gender = (UserGender)_dbUser.Gender,
                City = _dbUser.City,
                LastName = _dbUser.LastName,
                Status = (UserStatus)_dbUser.Status,
                IsAdmin = _dbUser.IsAdmin,
                Rate = _dbUser.Rate,
                StartWorkingAt = _dbUser.StartWorkingAt.ToString()
            };

            _departmentInfo = new DepartmentInfo
            {
                Id = Guid.NewGuid(),
                Name = "Department name"
            };

            _positionInfo = new PositionInfo
            {
                Id = Guid.NewGuid(),
                Name = "Position name"
            };

            _projects = new List<ProjectInfo>
            {
                new ProjectInfo
                {
                    Id = Guid.NewGuid(),
                    Name = "Project name",
                    Status = "Active"
                }
            };

            _avatarInfo = new ImageInfo
            {
                Id = _dbUser.AvatarFileId,
                Content = "Content",
                Extension = ".jpg",
                ParentId = null
            };

            _images = new List<ImageInfo>
            {
                _avatarInfo
            };

            _errors = new List<string>
            {
                "error"
            };

            _filter = new GetUserFilter
            {
                UserId = _dbUser.Id,
                Name = _dbUser.LastName,
                IncludeCommunications = true,
                IncludeAchievements = true,
                IncludeCertificates = true,
                IncludeDepartment = true,
                IncludeEducations = true,
                IncludeImages = true,
                IncludePosition = true,
                IncludeProjects = true,
                IncludeSkills = true
            };

            _response = new UserResponse
            {
                User = _userInfo,
                Skills = new List<string>
                {
                    _dbSkill.Name
                },
                Communications = new List<CommunicationInfo>
                {
                    _communicationInfo
                },
                Certificates = new List<CertificateInfo>
                {
                    _certificateInfo
                },
                Achievements = new List<UserAchievementInfo>
                {
                    _achievementInfo
                },
                Projects = _projects,
                Educations = new List<EducationInfo>
                {
                    _educationInfo
                }
            };
        }

        [SetUp]
        public void SetUp()
        {
            CreateModels();

            _mocker = new AutoMocker();
            _mapper = _mocker.CreateInstance<UserResponseMapper>();

            _mocker
                .Setup<IUserInfoMapper, UserInfo>(x => x.Map(_dbUser, _departmentInfo, _positionInfo, null, null, _avatarInfo, null))
                .Returns(_userInfo);

            _mocker
                .Setup<IUserAchievementInfoMapper, UserAchievementInfo>(x =>
                    x.Map(_dbUserAchievement, It.IsAny<ImageInfo>()))
                .Returns(_achievementInfo);

            _mocker
                .Setup<ICertificateInfoMapper, CertificateInfo>(x =>
                    x.Map(_dbUserCertificate, It.IsAny<ImageInfo>()))
                .Returns(_certificateInfo);

            _mocker
                .Setup<IEducationInfoMapper, EducationInfo>(x =>
                    x.Map(_dbUserEducation))
                .Returns(_educationInfo);
        }

        /*[Test]
        public void ShouldReturnFullCorrectResponse()
        {
            SerializerAssert.AreEqual(
                _response,
                _mapper.Map(
                    _dbUser,
                    _departmentInfo,
                    _positionInfo,
                    null,
                    null,
                    _projects,
                    _images,
                    _filter));
        }*/

        //[Test]
        //public void ShouldReturnCorrectResponseWithoutOptionalFields()
        //{
        //    _filter = new GetUserFilter
        //    {
        //        UserId = _dbUser.Id,
        //        Name = _dbUser.LastName
        //    };


        //    var avatar = new ImageInfo
        //    {
        //        Id = _dbUser.AvatarFileId
        //    };

        //    var response = new UserResponse
        //    {
        //        User = _userInfo,
        //        Errors = _errors
        //    };

        //    SerializerAssert.AreEqual(
        //        response,
        //        _mapper.Map(
        //            _dbUser,
        //            null,
        //            null,
        //            null,
        //            null,
        //            _filter,
        //            _errors));
        //}

        /*[Test]
        public void ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _mapper.Map(
                    null,
                    _departmentInfo,
                    _positionInfo,
                    null,
                    null,
                    _projects,
                    _images,
                    _filter));
        }*/
    }
}
