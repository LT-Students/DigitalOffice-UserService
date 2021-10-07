using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
  public class ProjectInfoMapper : IProjectInfoMapper
  {
    public ProjectInfo Map(ProjectData projectData)
    {
      if (projectData == null)
      {
        return null;
      }

      return new ProjectInfo
      {
        Id = projectData.Id,
        Name = projectData.Name,
        ShortDescription = projectData.ShortDescription,
        ShortName = projectData.ShortName,
        Status = projectData.Status
      };
    }
  }
}
