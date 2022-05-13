using LT.DigitalOffice.Models.Broker.Models.Project;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
  public class ProjectInfoMapper : IProjectInfoMapper
  {
    public ProjectInfo Map(ProjectData projectData, ProjectUserData projectUser)
    {
      return projectData is null
        ? default
        : new ProjectInfo
        {
          Id = projectData.Id,
          Name = projectData.Name,
          ShortDescription = projectData.ShortDescription,
          ShortName = projectData.ShortName,
          Status = projectData.Status,
          User = projectUser is null
            ? default
            : new ProjectUserInfo()
            {
              IsActive = projectUser.IsActive,
              CreatedAtUtc = projectUser.CreatedAtUtc
            }
        };
    }
  }
}
