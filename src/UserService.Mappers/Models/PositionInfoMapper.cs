using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
  public class PositionInfoMapper : IPositionInfoMapper
  {
    public PositionInfo Map(PositionData position)
    {
      return position is null
        ? default
        : new PositionInfo
        {
          Id = position.Id,
          Name = position.Name
        };
    }
  }
}
