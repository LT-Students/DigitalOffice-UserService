using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Achievement.Filters;

namespace LT.DigitalOffice.UserService.Business.Commands.Achievement.Interfaces
{
  [AutoInject]
  public interface IFindAchievementCommand
  {
    FindResultResponse<AchievementInfo> ExecuteAsync(FindAchievementFilter filter);
  }
}
