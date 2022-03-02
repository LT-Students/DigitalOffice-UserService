using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.PendingUser.Filters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
  [AutoInject]
  public interface IPendingUserRepository
  {
    Task CreateAsync(DbPendingUser dbPendingUser);

    Task<DbPendingUser> GetAsync(Guid userId, bool includeUser = false);

    Task UpdateAsync(DbPendingUser dbPendingUser);

    Task<(List<DbPendingUser> dbPendingUsers, int totalCount)> FindAsync(FindPendingUserFilter filter);

    Task<DbPendingUser> RemoveAsync(Guid userId);

    Task<bool> DoesExistAsync(Guid userId);
  }
}
