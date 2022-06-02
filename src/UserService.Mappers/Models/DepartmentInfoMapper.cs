using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
  public class DepartmentInfoMapper : IDepartmentInfoMapper
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DepartmentInfoMapper(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    public DepartmentInfo Map(DepartmentData department)
    {
      return department is null
        ? default
        : new DepartmentInfo
        {
          Id = department.Id,
          Name = department.Name,
          ShortName = department.ShortName,
          ProjectsIds = department.Users.FirstOrDefault(user => (user.UserId == _httpContextAccessor.HttpContext.GetUserId()) && (user.Role == DepartmentUserRole.Manager)) != null
            ? department.ProjectsIds 
            : null
        };
    }
  }
}
