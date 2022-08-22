using LT.DigitalOffice.Models.Broker.Models.Office;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
  public class OfficeInfoMapper : IOfficeInfoMapper
  {
    public OfficeInfo Map(OfficeData office)
    {
      return office is null
        ? default
        : new OfficeInfo
        {
          Id = office.Id,
          Name = office.Name,
          Address = office.Address,
          City = office.City,
          Longitude = office.Longitude,
          Latitude = office.Latitude
        };
    }
  }
}
