using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Responses.User;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Mappers.Responses.Interfaces
{
  [AutoInject]
  public interface IUserResponseMapper
  {
    UserResponse Map(
      DbUser dbUser,
      CompanyUserInfo companyUser,
      ImageInfo avatar,
      DepartmentUserInfo departmentUser,
      List<EducationInfo> educations,
      List<ImageInfo> images,
      OfficeInfo office,
      PositionInfo position,
      List<ProjectInfo> projects,
      RoleInfo role,
      List<SkillInfo> skills);
  }
}
