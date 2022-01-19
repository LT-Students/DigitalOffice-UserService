using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data
{
  public class PendingUserRepository : IPendingUserRepository
  {
    private readonly IDataProvider _provider;

    public PendingUserRepository(
      IDataProvider provider)
    {
      _provider = provider;
    }

    public async Task CreateAsync(DbPendingUser dbPendingUser)
    {
      _provider.PendingUsers.Add(dbPendingUser);
      await _provider.SaveAsync();
    }

    public async Task<DbPendingUser> GetAsync(Guid userId)
    {
      return await _provider.PendingUsers
        .FirstOrDefaultAsync(pu => pu.UserId == userId);
    }

    public async Task RemoveAsync(Guid userId)
    {
      DbPendingUser dbPendingUser = await _provider.PendingUsers
        .FirstOrDefaultAsync(pu => pu.UserId == userId);

      if (dbPendingUser != null)
      {
        _provider.PendingUsers.Remove(dbPendingUser);
        await _provider.SaveAsync();
      }
    }

    public async Task<bool> DoesExistAsync(Guid userId)
    {
      return await _provider.PendingUsers.AnyAsync(pu => pu.UserId == userId);
    }
  }
}
