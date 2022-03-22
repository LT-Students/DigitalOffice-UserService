using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Department;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface IDepartmentService
  {
    Task CreateDepartmentUserAsync(Guid departmentId, Guid userId, List<string> errors);

    Task<List<DepartmentData>> GetDepartmentsAsync(Guid userId, List<string> errors);
  }
}
