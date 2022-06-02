using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
  public class DepartmentInfoMapper : IDepartmentInfoMapper
  {
    public DepartmentInfo Map(DepartmentData department)
    {
      return department is null
        ? default
        : new DepartmentInfo
        {
          Id = department.Id,
          Name = department.Name,
          ShortName = department.ShortName
        };
    }
  }
}
