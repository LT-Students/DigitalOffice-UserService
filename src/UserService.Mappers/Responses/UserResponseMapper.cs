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
  /// <inheritdoc />
  public class UserResponseMapper : IUserResponseMapper
  {
    private readonly IUserInfoMapper _userInfoMapper;
    private readonly IUserAchievementInfoMapper _userAchievementInfoMapper;
    private readonly ICertificateInfoMapper _certificateInfoMapper;
    private readonly IEducationInfoMapper _educationInfoMapper;

    private ImageInfo GetImage(List<ImageInfo> images, Guid? imageId)
    {
      if (images == null || !images.Any() || !imageId.HasValue)
      {
        return null;
      }

      return images.FirstOrDefault(
          i =>
              i.ParentId == imageId ||
              i.Id == imageId);
    }

    private List<ImageInfo> GetImages(List<ImageInfo> images, List<Guid> imagesIds)
    {
      if (images == null || !images.Any() || imagesIds == null || !imagesIds.Any())
      {
        return null;
      }

      return images.Where(x => imagesIds.Contains(x.Id.Value)).ToList();
    }

    public UserResponseMapper(
        IUserInfoMapper userInfoMapper,
        IUserAchievementInfoMapper userAchievementInfoMapper,
        ICertificateInfoMapper certificateInfoMapper,
        IEducationInfoMapper educationInfoMapper)
    {
      _userInfoMapper = userInfoMapper;
      _userAchievementInfoMapper = userAchievementInfoMapper;
      _certificateInfoMapper = certificateInfoMapper;
      _educationInfoMapper = educationInfoMapper;
    }

    public UserResponse Map(
        DbUser dbUser,
        DepartmentInfo department,
        PositionInfo position,
        OfficeInfo office,
        RoleInfo role,
        List<ProjectInfo> projects,
        List<ImageInfo> images,
        ImageInfo avatar,
        List<Guid> userImagesIds,
        GetUserFilter filter)
    {
      if (dbUser == null)
      {
        return null;
      }

      return new UserResponse
      {
          User = _userInfoMapper.Map(dbUser, department, position, avatar, role, office),
          Projects = projects,
          Skills = filter.IncludeSkills
              ? dbUser.Skills.Select(s => s.Skill.Name)
              : null,
          Achievements = filter.IncludeAchievements
              ? dbUser.Achievements.Select(ua => _userAchievementInfoMapper.Map(ua))
              : null,
          Certificates = filter.IncludeCertificates
              ? dbUser.Certificates.Select(
                  c =>
                      _certificateInfoMapper.Map(c, GetImage(images, c.ImageId)))
              : null,
          Communications = filter.IncludeCommunications
              ? dbUser.Communications.Select(
                  c => new CommunicationInfo
                  {
                      Id = c.Id,
                      Type = (CommunicationType)c.Type,
                      Value = c.Value
                  })
              : null,
          Educations = filter.IncludeEducations
              ? dbUser.Educations.Select(
                  e => _educationInfoMapper.Map(e))
              : null
      };
    }
  }
}
