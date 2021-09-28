using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Achievement;

namespace LT.DigitalOffice.UserService.Mappers.Responses.Interfaces
{
  [AutoInject]
  public interface IAchievementResponseMapper
  {
    AchievementResponse Map(DbAchievement dbAchievement);
  }
}
