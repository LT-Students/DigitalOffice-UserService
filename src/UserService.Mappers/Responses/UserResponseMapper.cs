using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters;
using LT.DigitalOffice.UserService.Models.Dto.Responses.User;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers.Responses
{
  public class UserResponseMapper : IUserResponseMapper
  {
    private readonly IUserInfoMapper _userInfoMapper;
    private readonly IUserAchievementInfoMapper _userAchievementInfoMapper;
    //private readonly ICertificateInfoMapper _certificateInfoMapper;
    //private readonly IEducationInfoMapper _educationInfoMapper;

    public UserResponseMapper(
      IUserInfoMapper userInfoMapper,
      IUserAchievementInfoMapper userAchievementInfoMapper)
      //ICertificateInfoMapper certificateInfoMapper,
      //IEducationInfoMapper educationInfoMapper
      //)
    {
      _userInfoMapper = userInfoMapper;
      _userAchievementInfoMapper = userAchievementInfoMapper;
      //_certificateInfoMapper = certificateInfoMapper;
      //_educationInfoMapper = educationInfoMapper;
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
      RoleInfo role)
    {
      if (dbUser == null)
      {
        return null;
      }

      return new UserResponse
      {
        User = _userInfoMapper.Map(
          dbUser,
          companyUserData,
          avatar,
          company,
          department,
          office,
          position,
          role),
        Images = images,
        Achievements = dbUser.Achievements?.Select(ua => _userAchievementInfoMapper.Map(ua)),
        Certificates = certificates,
        Communications = dbUser.Communications?.Select(
          c => new CommunicationInfo
          {
            Id = c.Id,
            Type = (CommunicationType)c.Type,
            Value = c.Value
          }),
        Educations = educations,
        Projects = projects,
        Skills = dbUser.Skills.Select(s => s.Skill.Name)
      };
    }
  }
}
