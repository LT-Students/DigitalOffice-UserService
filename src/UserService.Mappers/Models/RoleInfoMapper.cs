using LT.DigitalOffice.Models.Broker.Models.Right;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
  public class RoleInfoMapper : IRoleInfoMapper
  {
    public RoleInfo Map(RoleData role)
    {
      if (role == null)
      {
        return null;
      }

      return new RoleInfo
      {
        Id = role.Id,
        Name = role.Name,
        RightsIds = role.RightsIds
      };
    }
  }
}
