using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Company;
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
      CompanyUserData companyUserData,
      ImageInfo avatar,
      List<CertificateInfo> certificates,
      CompanyInfo company,
      DepartmentInfo department,
      List<EducationInfo> educations,
      List<ImageInfo> images,
      OfficeInfo office,
      PositionInfo position,
      List<ProjectInfo> projects,
      RoleInfo role,
      List<UserSkillInfo> skills);
  }
}
