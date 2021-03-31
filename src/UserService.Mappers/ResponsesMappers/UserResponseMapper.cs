using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UserService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers.ResponsesMappers
{
    public class UserResponseMapper : IUserResponseMapper
    {
        public User Map(DbUser dbUser)
        {
            if (dbUser == null)
            {
                throw new BadRequestException();
            }

            return new User
            {
                Id = dbUser.Id,
                AchievementsIds = dbUser.Achievements?.Select(dbUserAchievement => new Achievement
                {
                    Id = dbUserAchievement.Achievement.Id,
                    Message = dbUserAchievement.Achievement.Message,
                    PictureFileId = dbUserAchievement.Achievement.PictureFileId
                }).ToList(),
                AvatarFileId = dbUser.AvatarFileId,
                CertificatesIds = dbUser.CertificatesFiles?.Select(x => x.CertificateId).ToList(),
                FirstName = dbUser.FirstName,
                LastName = dbUser.LastName,
                MiddleName = dbUser.MiddleName,
                Status = (UserStatus)dbUser.Status,
                IsAdmin = dbUser.IsAdmin,
                CreatedAt = dbUser.CreatedAt,
                Communications = dbUser.Communications?.Select(
                    c =>
                        new Communications
                        {
                            Type = (CommunicationType)c.Type,
                            Value = c.Value
                        }) ?? new List<Communications>()
            };
        }
    }
}
