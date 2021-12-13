using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Models;

namespace LT.DigitalOffice.UserService.Mappers.Models.Interfaces
{
  [AutoInject]
  public interface IUserInfoMapper
  {
    UserInfo Map(
      DbUser dbUser,
      CompanyUserData companyUserData,
      ImageInfo avatar,
      CompanyInfo company,
      DepartmentInfo department,
      OfficeInfo office,
      PositionInfo position,
      RoleInfo role);
  }
}
