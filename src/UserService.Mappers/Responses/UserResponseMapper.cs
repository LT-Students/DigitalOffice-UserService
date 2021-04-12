using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters;
using LT.DigitalOffice.UserService.Models.Dto.Responses.User;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers.Responses
{
    /// <inheritdoc />
    public class UserResponseMapper : IUserResponseMapper
    {
        private readonly IUserInfoMapper _userInfoMapper;
        private readonly IUserAchievementInfoMapper _userAchievementInfoMapper;
        private readonly ICertificateInfoMapper _certificateInfoMapper;

        private ImageInfo GetImage(List<ImageInfo> images, Guid? imageId)
        {
            if (images == null || !images.Any() || !imageId.HasValue)
            {
                return null;
            }

            return images.FirstOrDefault(
                i =>
                    i.ParentId == imageId ||
                    i.Id == imageId);
        }

        public UserResponseMapper(
            IUserInfoMapper userInfoMapper,
            IUserAchievementInfoMapper userAchievementInfoMapper,
            ICertificateInfoMapper certificateInfoMapper)
        {
            _userInfoMapper = userInfoMapper;
            _userAchievementInfoMapper = userAchievementInfoMapper;
            _certificateInfoMapper = certificateInfoMapper;
        }

        public UserResponse Map(
            DbUser dbUser,
            DepartmentInfo department,
            PositionInfo position,
            List<ProjectInfo> projects,
            List<ImageInfo> images,
            GetUserFilter filter,
            List<string> errors)
        {
            if (dbUser == null)
            {
                throw new ArgumentNullException(nameof(dbUser));
            }

            ImageInfo avatar = new()
            {
                Id = dbUser.AvatarFileId
            };

            if (images != null && images.Any())
            {
                avatar = images.FirstOrDefault(i => i.Id == avatar.Id) ?? avatar;
            }

            return new UserResponse
            {
                User = _userInfoMapper.Map(dbUser),
                Avatar = avatar,
                Department = department,
                Position = position,
                Projects = projects,
                Skills = filter.IsIncludeSkills
                    ? dbUser.Skills.Select(s => s.Skill.SkillName)
                    : null,
                Achievements = filter.IsIncludeAchievements
                    ? dbUser.Achievements.Select(
                        ua =>
                            _userAchievementInfoMapper.Map(ua, GetImage(images, ua.Achievement?.ImageId)))
                    : null,
                Certificates = filter.IsIncludeCertificates
                    ? dbUser.Certificates.Select(
                        c =>
                            _certificateInfoMapper.Map(c, GetImage(images, c.ImageId)))
                    : null,
                Communications = filter.IsIncludeCommunications
                    ? dbUser.Communications.Select(
                        c => new CommunicationInfo
                        {
                            Type = (CommunicationType)c.Type,
                            Value = c.Value
                        })
                    : null,
                Errors = errors
            };
        }
    }
}
