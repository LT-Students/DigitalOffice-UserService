using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.UserService.Models.Dto.Models;

namespace LT.DigitalOffice.UserService.Mappers.Models.Interfaces
{
    [AutoInject]
    public interface IOfficeInfoMapper
    {
        OfficeInfo Map(OfficeData office);
    }
}
