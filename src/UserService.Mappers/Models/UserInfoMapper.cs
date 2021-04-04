using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
    public class UserInfoMapper : IUserInfoMapper
    {
        public UserInfo Map(DbUser dbUser)
        {
            if (dbUser == null)
            {
                throw new ArgumentNullException(nameof(dbUser));
            }

            return new UserInfo
            {
                Id = dbUser.Id,
                FirstName = dbUser.FirstName,
                LastName = dbUser.LastName,
                MiddleName = dbUser.MiddleName,
                StartWorkingAt = dbUser.StartWorkingAt.ToShortDateString(),
                About = dbUser.About,
                IsAdmin = dbUser.IsAdmin,
                Status = (UserStatus)dbUser.Status
            };
        }
    }
}
