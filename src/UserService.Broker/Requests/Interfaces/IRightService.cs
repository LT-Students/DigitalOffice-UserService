using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface IRightService
  {
    Task CreateUserRoleAsync(Guid roleId, Guid userId, List<string> errors);

    Task<List<RoleData>> GetRolesAsync(
      Guid userId,
      string locale,
      List<string> errors);
  }
}
