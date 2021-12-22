using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Models;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
  public class UserInfoMapper : IUserInfoMapper
  {
    public UserInfo Map(
      DbUser dbUser,
      CompanyUserData companyUserData,
      ImageInfo avatar,
      CompanyInfo company,
      DepartmentInfo department,
      OfficeInfo office,
      PositionInfo position,
      RoleInfo role)
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
        Status = (UserStatus)dbUser.Status,
        Rate = companyUserData?.Rate,
        StartWorkingAt = companyUserData?.StartWorkingAt?.ToShortDateString(),
        IsAdmin = dbUser.IsAdmin,
        IsActive = dbUser.IsActive,
        Avatar = avatar,
        Company = company,
        Department = department,
        Office = office,
        Position = position,
        Role = role,
      };
    }
  }
}
