using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Mappers.Models.Interfaces
{
  [AutoInject]
  public interface IUserInfoMapper
  {
    UserInfo Map(DbUser dbUser,
      DepartmentInfo department,
      CompanyInfo company,
      CompanyUserData companyUserData,
      PositionInfo position,
      OfficeInfo office,
      RoleInfo role,
      ImageInfo avatarImage,
      List<ImageInfo> images = null);
  }
}
