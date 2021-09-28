using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Achievement;
using Microsoft.AspNetCore.JsonPatch;
using System;

namespace LT.DigitalOffice.UserService.Business.Commands.Achievement.Interfaces
{
  [AutoInject]
  public interface IEditAchievementCommand
  {
    OperationResultResponse<bool> Execute(Guid achievementId, JsonPatchDocument<EditAchievementRequest> request);
  }
}
