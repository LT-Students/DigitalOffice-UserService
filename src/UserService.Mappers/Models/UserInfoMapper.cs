using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
    public class UserInfoMapper : IUserInfoMapper
    {
        public UserInfo Map(DbUser dbUser,
            DepartmentInfo department,
            PositionInfo position,
            ImageInfo image,
            RoleInfo role,
            OfficeInfo office)
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
                Gender = (UserGender)dbUser.Gender,
                DateOfBirth = dbUser.DateOfBirth?.ToShortDateString(),
                City = dbUser.City,
                StartWorkingAt = dbUser.StartWorkingAt?.ToShortDateString(),
                IsAdmin = dbUser.IsAdmin,
                IsActive = dbUser.IsActive,
                Status = (UserStatus)dbUser.Status,
                Rate = dbUser.Rate,
                Office = office,
                Role = role,
                Avatar = image,
                Department = department,
                Position = position
            };
        }
    }
}
