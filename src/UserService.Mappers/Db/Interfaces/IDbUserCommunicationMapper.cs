using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Communication;

namespace LT.DigitalOffice.UserService.Mappers.Db.Interfaces
{
  [AutoInject]
  public interface IDbUserCommunicationMapper
  {
    DbUserCommunication Map(CreateCommunicationRequest request);
  }
}
