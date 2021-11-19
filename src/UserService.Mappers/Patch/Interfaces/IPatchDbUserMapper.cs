using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.UserService.Mappers.Patch.Interfaces
{
  [AutoInject]
  public interface IPatchDbUserMapper
  {
    (JsonPatchDocument<DbUser> dbUserPatch, JsonPatchDocument<DbUserLocation> dbUserLocationPatch) Map(
      JsonPatchDocument<EditUserRequest> request);
  }
}