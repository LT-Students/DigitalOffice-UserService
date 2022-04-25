using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Project;
using LT.DigitalOffice.UserService.Models.Dto.Models;

namespace LT.DigitalOffice.UserService.Mappers.Models.Interfaces
{
  [AutoInject]
  public interface IProjectInfoMapper
  {
    ProjectInfo Map(ProjectData projectData, ProjectUserData projectUser);
  }
}
