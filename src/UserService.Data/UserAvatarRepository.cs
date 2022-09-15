using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Data.Provider;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data
{
  public class ImageRepository : IUserAvatarRepository
  {
    private readonly IDataProvider _provider;

    public ImageRepository(IDataProvider provider)
    {
      _provider = provider;
    }

    public async Task CreateAsync(DbUserAvatar dbUserAvatar)
    {
      if (dbUserAvatar is null)
      {
        return;
      }

      if (dbUserAvatar.IsCurrentAvatar)
      {
        DbUserAvatar currentUserAvatar = await _provider.UsersAvatars
          .FirstOrDefaultAsync(ua => dbUserAvatar.UserId == ua.UserId && ua.IsCurrentAvatar);

        if (currentUserAvatar is not null)
        {
          currentUserAvatar.IsCurrentAvatar = false;
        }
      }

      _provider.UsersAvatars.Add(dbUserAvatar);
      await _provider.SaveAsync();
    }

    public Task<List<Guid>> GetAvatarsByUserId(Guid avatarId, CancellationToken cancellationToken = default)
    {
      return _provider.UsersAvatars
        .Where(a => a.AvatarId == avatarId)
        .Select(a => a.AvatarId)
        .ToListAsync(cancellationToken);
    }

    public Task<List<DbUserAvatar>> GetAsync(List<Guid> imagesIds)
    {
      if (imagesIds is null || !imagesIds.Any())
      {
        return null;
      }

      return _provider.UsersAvatars
        .Where(a => imagesIds.Contains(a.AvatarId)).ToListAsync();
    }

    public async Task<bool> UpdateCurrentAvatarAsync(Guid userId, Guid avatarId)
    {
      DbUserAvatar dbUserAvatar = await _provider.UsersAvatars.FirstOrDefaultAsync(x => x.AvatarId == avatarId);

      if (dbUserAvatar is null || dbUserAvatar.UserId != userId)
      {
        return false;
      }

      DbUserAvatar currentUserAvatar = await _provider.UsersAvatars
        .FirstOrDefaultAsync(ua => dbUserAvatar.UserId == ua.UserId && ua.IsCurrentAvatar);

      if (currentUserAvatar is not null)
      {
        currentUserAvatar.IsCurrentAvatar = false;
      }

      dbUserAvatar.IsCurrentAvatar = true;
      await _provider.SaveAsync();

      return true;
    }

    public async Task<List<Guid>> RemoveAsync(Guid userId)
    {
      List<DbUserAvatar> removeUsersAvatars = await _provider.UsersAvatars
        .Where(ua => ua.UserId == userId).ToListAsync();

      _provider.UsersAvatars.RemoveRange(removeUsersAvatars);
      await _provider.SaveAsync();

      return removeUsersAvatars.Select(ua => ua.AvatarId).ToList();
    }

    public async Task<bool> RemoveAsync(List<Guid> imagesIds)
    {
      if (imagesIds is null || !imagesIds.Any())
      {
        return false;
      }

      List<DbUserAvatar> removeUsersAvatars = await GetAsync(imagesIds);

      _provider.UsersAvatars.RemoveRange(removeUsersAvatars);
      await _provider.SaveAsync();

      return true;
    }
  }
}
