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
        public User Map(DbUser dbUser)
        {
            if (dbUser == null)
            {
                throw new ArgumentNullException(nameof(dbUser));
            }

            return new User
            {
                Id = dbUser.Id,
                Achievements = dbUser.AchievementsIds?.Select(dbUserAchievement => new Achievement
                {
                    Id = dbUserAchievement.Achievement.Id,
                    Message = dbUserAchievement.Achievement.Message,
                    PictureFileId = dbUserAchievement.Achievement.PictureFileId
                }).ToList(),
                AvatarId = dbUser.AvatarFileId,
                CertificatesIds = dbUser.CertificatesFilesIds?.Select(x => x.CertificateId).ToList(),
                FirstName = dbUser.FirstName,
                LastName = dbUser.LastName,
                MiddleName = dbUser.MiddleName,
                Status = dbUser.Status,
                IsAdmin = dbUser.IsAdmin
            };
        }

        public DbUser Map(UserRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return new DbUser
            {
                Id = (Guid)request.Id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                MiddleName = request.MiddleName,
                Status = request.Status,
                AvatarFileId = request.AvatarFileId,
                IsActive = request.IsActive,
                IsAdmin = request.IsAdmin
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