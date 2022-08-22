using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.JsonPatch;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Filtres;
using System.Threading.Tasks;
using System.Linq;
using LT.DigitalOffice.UserService.Models.Dto.Enums;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
  [AutoInject]
  public interface IUserRepository
  {
    Task<DbUser> GetAsync(GetUserFilter filter);

    Task<DbUser> GetAsync(Guid userId);

    Task<List<Guid>> AreExistingIdsAsync(List<Guid> usersIds);

    Task<(List<DbUser> dbUsers, int totalCount)> FindAsync(FindUsersFilter filter, List<Guid> userIds = null);

    Task<Guid> CreateAsync(DbUser dbUser);

    Task<bool> EditUserAdditionAsync(Guid userId, JsonPatchDocument<DbUserAddition> patch);

    Task<bool> EditUserAsync(Guid id, JsonPatchDocument<DbUser> userPatch);

    Task<bool> SwitchActiveStatusAsync(Guid userId, bool isActive);

    IQueryable<DbUser> SearchAsync(string text, IQueryable<DbUser> dbUser = null);
  }
}