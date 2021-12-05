using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Achievement;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.UserService.Mappers.Patch.Interfaces
{
  [AutoInject]
  public interface IPatchDbAchievementMapper
  {
    JsonPatchDocument<DbAchievement> Map(JsonPatchDocument<EditAchievementRequest> request);
  }
}
