using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
    public class UserAchievementInfoMapper : IUserAchievementInfoMapper
    {
        public UserAchievementInfo Map(
            DbUserAchievement dbUserAchievement)
        {
            if (dbUserAchievement == null)
            {
                throw new ArgumentNullException(nameof(dbUserAchievement));
            }

            return new UserAchievementInfo
            {
                Id = dbUserAchievement.Id,
                AchievementId = dbUserAchievement.AchievementId,
                ReceivedAt = dbUserAchievement.ReceivedAt,
                Name = dbUserAchievement.Achievement.Name,
                Image = new ImageConsist
                {
                  Content = dbUserAchievement.Achievement.ImageContent,
                  Extension = dbUserAchievement.Achievement.ImageExtension
                }
            };
        }
    }
}
