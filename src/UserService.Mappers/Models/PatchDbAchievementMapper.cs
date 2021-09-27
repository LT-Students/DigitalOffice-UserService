using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Achievement;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
  public class PatchDbAchievementMapper : IPatchDbAchievementMapper
  {
    public JsonPatchDocument<DbAchievement> Map(JsonPatchDocument<EditAchievementRequest> request)
    {
      if (request == null)
      {
        return null;
      }

      JsonPatchDocument<DbAchievement> patchDbAchievement = new JsonPatchDocument<DbAchievement>();

      foreach (Operation<EditAchievementRequest> item in request.Operations)
      {
        patchDbAchievement.Operations.Add(new Operation<DbAchievement>(item.op, item.path, item.from, item.value));
      }

      return patchDbAchievement;
    }
  }
}
