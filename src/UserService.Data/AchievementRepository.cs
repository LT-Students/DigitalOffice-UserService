using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using System;

namespace LT.DigitalOffice.UserService.Data
{
  public class AchievementRepository : IAchievementRepository
  {
    private readonly IDataProvider _provider;

    public AchievementRepository(IDataProvider provider)
    {
      _provider = provider;
    }

    public Guid Create(DbAchievement dbAchievement)
    {
      _provider.Achievements.Add(dbAchievement);
      _provider.Save();

      return dbAchievement.Id;
    }
  }
}
