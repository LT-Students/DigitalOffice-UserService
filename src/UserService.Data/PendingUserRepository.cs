using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.PendingUser.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

    public async Task<DbPendingUser> GetAsync(Guid userId, bool includeUser)
    {
      IQueryable<DbPendingUser> dbPendingUser = _provider.PendingUsers.AsQueryable();

      if (includeUser)
      {
        dbPendingUser = dbPendingUser.Include(pu => pu.User).ThenInclude(u => u.Communications);
      }

      return await dbPendingUser
        .FirstOrDefaultAsync(pu => pu.UserId == userId);
    }

    public async Task UpdateAsync(DbPendingUser dbPendingUser)
    {
      _provider.PendingUsers.Update(dbPendingUser);
      await _provider.SaveAsync();
    }

    public async Task<(List<DbPendingUser> dbPendingUsers, int totalCount)> FindAsync(
      FindPendingUserFilter filter)
    {
      IQueryable<DbPendingUser> dbPendingUsers = _provider.PendingUsers.AsQueryable();

      dbPendingUsers = dbPendingUsers.Include(pu => pu.User);

      if (filter.IncludeCommunication)
      {
        dbPendingUsers = dbPendingUsers
          .Include(pu => pu.User)
          .ThenInclude(u => u.Communications);
      }

      if (filter.IncludeCurrentAvatar)
      {
        dbPendingUsers = dbPendingUsers
          .Include(pu => pu.User)
          .ThenInclude(u => u.Avatars.Where(ua => ua.IsCurrentAvatar));
      }

      return (
        await dbPendingUsers.Skip(filter.SkipCount).Take(filter.TakeCount).ToListAsync(),
        await dbPendingUsers.CountAsync());
    }

    public async Task<DbPendingUser> RemoveAsync(Guid userId)
    {
      DbPendingUser dbPendingUser = await _provider.PendingUsers
        .FirstOrDefaultAsync(pu => pu.UserId == userId);

      if (dbPendingUser is not null)
      {
        _provider.PendingUsers.Remove(dbPendingUser);
        await _provider.SaveAsync();
      }

      return dbPendingUser;
    }

    public async Task<bool> DoesExistAsync(Guid userId)
    {
      return await _provider.PendingUsers.AnyAsync(pu => pu.UserId == userId);
    }
  }
}
