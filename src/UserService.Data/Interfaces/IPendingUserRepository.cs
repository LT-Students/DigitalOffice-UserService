using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
  [AutoInject]
  public interface IPendingUserRepository
  {
    Task CreateAsync(DbPendingUser dbPendingUser);

    Task<DbPendingUser> GetAsync(Guid userId);

    Task RemoveAsync(Guid userId);

    Task<bool> DoesExistAsync(Guid userId);
  }
}
