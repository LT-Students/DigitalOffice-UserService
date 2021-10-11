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
  /// <summary>
  /// Represents interface of repository in repository pattern.
  /// Provides methods for working with the database of UserService.
  /// </summary>
  [AutoInject]
  public interface IUserRepository
  {
    DbUser Get(GetUserFilter filter);

    DbUser Get(Guid id);

    List<DbUser> Get(IEnumerable<Guid> userIds);

    List<Guid> AreExistingIds(List<Guid> userIds);

    (List<DbUser> dbUsers, int totalCount) Find(FindUsersFilter filter);

    DbPendingUser GetPendingUser(Guid userId);

    Task DeletePendingUserAsync(Guid userId);

    /// <summary>
    /// Adds new dbUser to the database. Returns whether it was successful to add.
    /// </summary>
    Task<Guid> CreateAsync(DbUser dbUser);

    Task CreatePendingAsync(DbPendingUser dbPendingUser);

    /// <summary>
    /// Edit existing dbUser. Returns whether it was successful to edit.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userPatch"></param>
    /// <returns>Whether it was successful to edit.</returns>
    Task<bool> EditUserAsync(Guid id, JsonPatchDocument<DbUser> userPatch);

    /// <summary>
    /// Return DbSkill if it exist in database, else return null.
    /// </summary>
    /// <param name="name">Skill name.</param>
    DbSkill FindSkillByName(string name);

    /// <summary>
    /// Adds new skill to Database. Returns Id of new DbSkill if it was successful to add.
    /// </summary>
    /// <param name="name">Skill name.</param>
    /// <returns> Guid of created DbSkill.</returns>
    Task<Guid> CreateSkillAsync(string name);

    /// <summary>
    /// Disable user.
    /// </summary>
    Task<bool> SwitchActiveStatusAsync(Guid userId, bool status);

    bool IsUserExist(Guid userId);

    bool IsCommunicationValueExist(List<string> value);

    List<DbUser> Search(string text);

    Task<bool> RemoveAvatarAsync(Guid userId);
  }
}