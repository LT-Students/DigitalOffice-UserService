using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Achievement;
using System;

namespace LT.DigitalOffice.UserService.Business.Commands.Achievement.Interfaces
{
  [AutoInject]
  public interface IGetAchievementCommand
  {
    OperationResultResponse<AchievementResponse> ExecuteAsync(Guid achievementId);
  }
}
