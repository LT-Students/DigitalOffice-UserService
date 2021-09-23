using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Achievement.Filters;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
  [AutoInject]
  public interface IAchievementRepository
  {
    Guid Create(DbAchievement dbAchievement);

    bool Edit(Guid achievementId, JsonPatchDocument<DbAchievement> request);

    DbAchievement Get(Guid achievementId);

    List<DbAchievement> Find(FindAchievementFilter filter, out int totalCount);
  }
}