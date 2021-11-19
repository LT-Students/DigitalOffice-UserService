using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
  public class UserInfoMapper : IUserInfoMapper
  {
    public UserInfo Map(
      DbUser dbUser,
      DepartmentInfo department,
      PositionInfo position,
      PositionUserData positionUserData,
      OfficeInfo office,
      RoleInfo role,
      ImageInfo avatarImage,
      List<ImageInfo> images = null)
    {
      if (dbUser is null)
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
        StartWorkingAt = dbUser.StartWorkingAt?.ToShortDateString(),
        IsAdmin = dbUser.IsAdmin,
        About = dbUser.About,
        IsActive = dbUser.IsActive,
        Status = (UserStatus)dbUser.Status,
        Rate = positionUserData?.Rate,
        Latitude = dbUser.Location?.Latitude,
        Longitude = dbUser.Location?.Latitude,
        BusinessHoursFromUtc = dbUser.Location?.BusinessHoursFromUtc,
        BusinessHoursToUtc = dbUser.Location?.BusinessHoursToUtc,
        Department = department,
        Position = position,
        Office = office,
        Role = role,
        Avatar = avatarImage,
        Images = images
      };
    }
  }
}
