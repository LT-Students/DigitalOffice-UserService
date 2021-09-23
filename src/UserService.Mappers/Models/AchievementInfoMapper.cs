using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Models;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
  public class AchievementInfoMapper : IAchievementInfoMapper
  {
    public AchievementInfo Map(DbAchievement dbAchievement)
    {
      if (dbAchievement == null)
      {
        return null;
      }

      return new AchievementInfo
      {
        Id = dbAchievement.Id,
        Name = dbAchievement.Name,
        Description = dbAchievement.Description,
        Image = new ImageConsist()
        {
          Content = dbAchievement.ImageContent,
          Extension = dbAchievement.ImageExtension
        },
        CreatedAtUtc = dbAchievement.CreatedAtUtc
      };
    }
  }
}