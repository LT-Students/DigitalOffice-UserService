using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
    public class UserInfoMapper : IUserInfoMapper
    {
        public UserInfo Map(DbUser dbUser, UsersDepartment department = null, UsersPosition position = null)
        {
            if (dbUser == null)
            {
                throw new ArgumentNullException(nameof(dbUser));
            }

            UserInfo response = new();

            if (department != null)
            {
                response.Department = new DepartmentInfo() { Id = department.Id, Name = department.Name };
            }

            if (position != null)
            {
                response.Position = new PositionInfo() { Id = position.Id, Name = position.Name };
            }

            response.Id = dbUser.Id;
            response.FirstName = dbUser.FirstName;
            response.LastName = dbUser.LastName;
            response.MiddleName = dbUser.MiddleName;
            response.Gender = (UserGender)dbUser.Gender;
            response.DateOfBirth = dbUser.DateOfBirth?.ToShortDateString();
            response.City = dbUser.City;
            response.StartWorkingAt = dbUser.DateOfBirth?.ToShortDateString();
            response.About = dbUser.About;
            response.IsAdmin = dbUser.IsAdmin;
            response.Status = (UserStatus)dbUser.Status;
            response.Rate = dbUser.Rate;

            return response;
        }
    }
}
