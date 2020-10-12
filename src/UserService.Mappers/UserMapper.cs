using LT.DigitalOffice.Kernel.Exceptions;
using LT.DigitalOffice.UserService.Mappers.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using System;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers
{
    /// <summary>
    /// Represents mapper. Provides methods for converting an object of <see cref="DbUser"/>
    /// type into an object of <see cref="User"/> type according to some rule.
    /// </summary>
    public class UserMapper : IMapper<DbUser, User>, IMapper<DbUser, string, object>,
        IMapper<UserRequest, DbUser>
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
                Achievements = value.AchievementsIds?.Select(dbUserAchievement => new Achievement
                {
                    Id = dbUserAchievement.Achievement.Id,
                    Message = dbUserAchievement.Achievement.Message,
                    PictureFileId = dbUserAchievement.Achievement.PictureFileId
                }).ToList(),
                AvatarId = value.AvatarFileId,
                CertificatesIds = value.CertificatesFilesIds?.Select(x => x.CertificateId).ToList(),
                FirstName = value.FirstName,
                LastName = value.LastName,
                MiddleName = value.MiddleName,
                Status = value.Status,
                IsAdmin = value.IsAdmin
            };
        }

        public DbUser Map(UserRequest value)
        {
            if (value == null)
            {
                throw new BadRequestException();
            }

            value.Id ??= Guid.NewGuid();

            return new DbUser
            {
                Id = value.Id.Value,
                Email = value.Email,
                FirstName = value.FirstName,
                LastName = value.LastName,
                MiddleName = value.MiddleName,
                Status = value.Status,
                AvatarFileId = value.AvatarFileId,
                IsActive = value.IsActive,
                IsAdmin = value.IsAdmin
            };
        }

        public object Map(DbUser user, string position)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(User));
            }

            if (position == null)
            {
                throw new ArgumentNullException(nameof(position));
            }

            return new
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                UserPosition = position
            };
        }
    }
}