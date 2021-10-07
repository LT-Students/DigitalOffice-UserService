using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;

namespace LT.DigitalOffice.UserService.Mappers.Models.Interfaces
{
  [AutoInject]
  public interface IUserAchievementInfoMapper
  {
    UserAchievementInfo Map(DbUserAchievement dbUserAchievement);
  }
}
