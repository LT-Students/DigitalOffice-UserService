using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Responses.User;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Mappers.Responses
{
  public class UserResponseMapper : IUserResponseMapper
  {
    private readonly IUserInfoMapper _userInfoMapper;

    public UserResponseMapper(
      IUserInfoMapper userInfoMapper)
    {
      _userInfoMapper = userInfoMapper;
    }

    public UserResponse Map(
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
      List<UserSkillInfo> skills)
    {
      if (dbUser is null)
      {
        return default;
      }

      return new UserResponse
      {
        User = _userInfoMapper.Map(dbUser, avatar),
        UserAddition = dbUser.Addition is null ? default : new()
        {
          GenderName = dbUser.Addition.Gender?.Name,
          About = dbUser.Addition.About,
          DateOfBirth = dbUser.Addition.DateOfBirth,
          Latitude = dbUser.Addition.Latitude,
          Longitude = dbUser.Addition.Latitude,
          BusinessHoursFromUtc = dbUser.Addition.BusinessHoursFromUtc,
          BusinessHoursToUtc = dbUser.Addition.BusinessHoursToUtc
        },
        Company = company,
        Department = department,
        Office = office,
        Position = position,
        Role = role,
        Images = images,
        Certificates = certificates,
        Educations = educations,
        Projects = projects,
        Skills = skills
      };
    }
  }
}
