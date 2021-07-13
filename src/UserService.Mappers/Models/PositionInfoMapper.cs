using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
    public class PositionInfoMapper : IPositionInfoMapper
    {
        public PositionInfo Map(PositionData position)
        {
            if (position == null)
            {
                return null;
            }

            return new PositionInfo
            {
                Id = position.Id,
                Name = position.Name
            };
        }
    }
}
