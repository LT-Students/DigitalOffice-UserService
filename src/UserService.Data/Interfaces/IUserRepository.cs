using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Filtres;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
  [AutoInject]
  public interface IUserRepository
  {
    Task<DbUser> GetAsync(GetUserFilter filter, CancellationToken cancellationToken = default);

    Task<DbUser> GetAsync(Guid userId);

    Task<List<Guid>> AreExistingIdsAsync(List<Guid> usersIds);

    Task<(List<DbUser> dbUsers, int totalCount)> FindAsync(FindUsersFilter filter, List<Guid> userIds = null, CancellationToken cancellationToken = default);

    Task CreateAsync(DbUser dbUser);

    Task<bool> EditUserAdditionAsync(Guid userId, JsonPatchDocument<DbUserAddition> patch);

    Task<bool> EditUserAsync(Guid id, JsonPatchDocument<DbUser> userPatch);

    Task<bool> SwitchActiveStatusAsync(Guid userId, bool isActive);

    IQueryable<DbUser> SearchAsync(string text, IQueryable<DbUser> dbUser = null);
  }
}