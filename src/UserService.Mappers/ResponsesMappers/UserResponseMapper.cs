using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.UserService.Mappers.ResponsesMappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers.ResponsesMappers
{
    public class UserResponseMapper : IUserResponseMapper
    {
        public User Map(DbUser value)
        {
            if (value == null)
            {
                throw new BadRequestException();
            }

            return new User
            {
                Id = value.Id,
                Email = value.Email,
                AchievementsIds = value.Achievements?.Select(dbUserAchievement => new Achievement
                {
                    Id = dbUserAchievement.Achievement.Id,
                    Message = dbUserAchievement.Achievement.Message,
                    PictureFileId = dbUserAchievement.Achievement.PictureFileId
                }).ToList(),
                AvatarFileId = value.AvatarFileId,
                CertificatesIds = value.CertificatesFiles?.Select(x => x.CertificateId).ToList(),
                FirstName = value.FirstName,
                LastName = value.LastName,
                MiddleName = value.MiddleName,
                Status = (UserStatus)value.Status,
                IsAdmin = value.IsAdmin,
                CreatedAt = value.CreatedAt
            };
        }
    }
}
