using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.UserService.Mappers.Patch.interfaces
{
  public interface IPatchDbUserAdditionMapper
  {
    JsonPatchDocument<DbUserAddition> Map(
      JsonPatchDocument<EditUserRequest> request);
  }
}
