using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using System;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
  public class DepartmentUserInfoMapper : IDepartmentInfoMapper
  {
    public DepartmentUserInfo Map(Guid userId, DepartmentData department)
    {
      DepartmentUserData user = department?.Users?.FirstOrDefault(user => user.UserId == userId);

      return department is null || user is null
        ? default
        : new DepartmentUserInfo
        {
          Department = new DepartmentInfo
          {
            Id = department.Id,
            Name = department.Name,
            ShortName = department.ShortName,
            ChildDepartmentsIds = department.ChildDepartmentsIds
          },
          Role = user.Role
        };
    }
  }
}
