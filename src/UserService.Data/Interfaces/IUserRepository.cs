using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.JsonPatch;

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

        IEnumerable<DbUser> Get(IEnumerable<Guid> userIds);

        IEnumerable<DbUser> Find(int skipCount, int takeCount, out int totalCount);

        DbPendingUser GetPendingUser(Guid userId);

        void DeletePendingUser(Guid userId);

        /// <summary>
        /// Adds new dbUser to the database. Returns whether it was successful to add.
        /// </summary>
        Guid Create(DbUser dbUser, string password);

        /// <summary>
        /// Edit existing dbUser. Returns whether it was successful to edit.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userPatch"></param>
        /// <returns>Whether it was successful to edit.</returns>
        bool EditUser(Guid id, JsonPatchDocument<DbUser> userPatch);

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
        Guid CreateSkill(string name);

        /// <summary>
        /// Disable user.
        /// </summary>
        bool SwitchActiveStatus(Guid userId, bool status);
    }
}