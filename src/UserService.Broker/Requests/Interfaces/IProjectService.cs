using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Project;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface IProjectService
  {
    Task<List<ProjectData>> GetProjectsAsync(Guid userId, List<string> errors, bool includeUsers = true, bool? acsendingSort = true);
  }
}
