﻿using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Models;

namespace LT.DigitalOffice.UserService.Mappers.Models.Interfaces
{
    [AutoInject]
    public interface IUserAchievementInfoMapper
    {
        UserAchievementInfo Map(
            DbUserAchievement dbUserAchievement,
            ImageInfo image);
    }
}
