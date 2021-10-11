using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Achievement.Filters;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
  [AutoInject]
  public interface IAchievementRepository
  {
    Task<Guid> Create(DbAchievement dbAchievement);

    Task<bool> Edit(Guid achievementId, JsonPatchDocument<DbAchievement> request);

    DbAchievement Get(Guid achievementId);

    List<DbAchievement> Find(FindAchievementFilter filter, out int totalCount);
  }
}