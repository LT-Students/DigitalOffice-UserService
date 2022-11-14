using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Requests.UserCompany;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Publishes.Interfaces
{
  [AutoInject]
  public interface IPublish
  {
    Task DisactivateUserAsync(Guid userId);

    Task ActivateUserAsync(Guid userId);

    Task CreatePendingUserAsync(Guid userId);

    Task RemoveImagesAsync(List<Guid> imagesIds);

    Task CreateUserOfficeAsync(Guid userId, Guid officeId);

    Task CreateUserRoleAsync(Guid userId, Guid roleId);

    Task CreateDepartmentUserAsync(Guid userId, Guid departmentId);

    Task CreateUserPositionAsync(Guid userId, Guid positionId);

    Task CreateCompanyUserAsync(Guid userId, CreateUserCompanyRequest userCompany);
  }
}
