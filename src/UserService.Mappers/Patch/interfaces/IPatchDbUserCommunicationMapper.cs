using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Communication;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.UserService.Mappers.Patch.Interfaces
{
  [AutoInject]
  public interface IPatchDbUserCommunicationMapper
  {
    JsonPatchDocument<DbUserCommunication> Map(
      JsonPatchDocument<EditCommunicationRequest> request);
  }
}
