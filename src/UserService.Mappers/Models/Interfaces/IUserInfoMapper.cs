using LT.DigitalOffice.Kernel.Attributes;
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
      PositionInfo position,
      OfficeInfo office,
      RoleInfo role,
      ImageInfo image,
      List<ImageInfo> images = null);
  }
}
