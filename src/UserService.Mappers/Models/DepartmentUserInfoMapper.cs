using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
  public class DepartmentUserInfoMapper : IDepartmentInfoMapper
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DepartmentUserInfoMapper(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    public DepartmentUserInfo Map(DepartmentData department)
    {
      DepartmentUserData user = department?.Users?.FirstOrDefault(user => user.UserId == _httpContextAccessor.HttpContext.GetUserId());

      return department is null || user is null
        ? default
        : new DepartmentUserInfo
        {
          Department = new DepartmentInfo
          {
            Id = department.Id,
            Name = department.Name,
            ShortName = department.ShortName
          },
          Role = user.Role
        };
    }
  }
}
