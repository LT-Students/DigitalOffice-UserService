using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Achievement.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Data
{
  public class AchievementRepository : IAchievementRepository
  {
    private readonly IDataProvider _provider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AchievementRepository(
      IDataProvider provider,
      IHttpContextAccessor httpContextAccessor)
    {
      _provider = provider;
      _httpContextAccessor = httpContextAccessor;
    }

    public Guid Create(DbAchievement dbAchievement)
    {
      _provider.Achievements.Add(dbAchievement);
      _provider.Save();

      return dbAchievement.Id;
    }

    public bool Edit(Guid achievementId, JsonPatchDocument<DbAchievement> request)
    {
      DbAchievement dbAchievement = _provider.Achievements.FirstOrDefault(x => x.Id == achievementId);

      if (dbAchievement == null || request == null)
      {
        return false;
      }

      request.ApplyTo(dbAchievement);
      dbAchievement.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
      dbAchievement.ModifiedAtUtc = DateTime.UtcNow;
      _provider.Save();

      return true;
    }

    public List<DbAchievement> Find(FindAchievementFilter filter, out int totalCount)
    {
      if (filter == null)
      {
        totalCount = 0;
        return null;
      }

      IQueryable<DbAchievement> dbNewsList = _provider.Achievements.AsQueryable();

      totalCount = dbNewsList.Count();

      return dbNewsList.Skip(filter.skipCount).Take(filter.takeCount).ToList();
    }

    public DbAchievement Get(Guid achievementId)
    {
      return _provider.Achievements.FirstOrDefault(dbAchievement => dbAchievement.Id == achievementId);
    }
  }
}
