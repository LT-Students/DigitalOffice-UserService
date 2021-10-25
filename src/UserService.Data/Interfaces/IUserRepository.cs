using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.JsonPatch;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Filtres;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
  [AutoInject]
  public interface IUserRepository
  {
    Task<DbUser> GetAsync(GetUserFilter filter);

    Task<DbUser> GetAsync(Guid id);

    Task<List<DbUser>> GetAsync(List<Guid> usersIds);

    Task<List<(DbUser user, Guid avatarId)>> GetWithAvatarsAsync(List<Guid> usersIds);

    Task<List<Guid>> AreExistingIdsAsync(List<Guid> usersIds);

    Task<(List<DbUser> dbUsers, int totalCount)> FindAsync(FindUsersFilter filter);

    Task<DbPendingUser> GetPendingUserAsync(Guid userId);

    Task DeletePendingUserAsync(Guid userId);

    Task<Guid> CreateAsync(DbUser dbUser);

    Task CreatePendingAsync(DbPendingUser dbPendingUser);

    Task<bool> EditUserAsync(Guid id, JsonPatchDocument<DbUser> userPatch);

    DbSkill FindSkillByName(string name);

    Task<Guid> CreateSkillAsync(string name);

    Task<bool> SwitchActiveStatusAsync(Guid userId, bool status);

    Task<bool> IsUserExistAsync(Guid userId);

    Task<bool> IsCommunicationValueExist(List<string> value);

    Task<List<DbUser>> SearchAsync(string text);
  }
}