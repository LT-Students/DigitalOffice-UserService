using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
  public class UserInfoMapper : IUserInfoMapper
  {
    public UserInfo Map(DbUser dbUser,
      DepartmentInfo department,
      PositionInfo position,
      OfficeInfo office,
      RoleInfo role,
      ImageInfo image,
      List<ImageInfo> images = null)
    {
      if (dbUser == null)
      {
        return null;
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
        About = dbUser.About,
        IsActive = dbUser.IsActive,
        Status = (UserStatus)dbUser.Status,
        Rate = dbUser.Rate,
        Department = department,
        Position = position,
        Office = office,
        Role = role,
        Avatar = image,
        Images = images
      };
    }
  }
}
