using LT.DigitalOffice.UserService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Achievement;
namespace LT.DigitalOffice.UserService.Mappers.Responses
{
  public class AchievementResponseMapper : IAchievementResponseMapper
  {
    public AchievementResponse Map(DbAchievement dbAchievement)
    {
      if (dbAchievement == null)
      {
        return null;
      }

      return new AchievementResponse
      {
        Id = dbAchievement.Id,
        Name = dbAchievement.Name,
        Description = dbAchievement.Description,
        Image = new ImageConsist()
        {
          Content = dbAchievement.ImageContent,
          Extension = dbAchievement.ImageExtension
        },
        CreatedAtUtc = dbAchievement.CreatedAtUtc,
        CreatedBy = dbAchievement.CreatedBy
      };
    }
  }
}
