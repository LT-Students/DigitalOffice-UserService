using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

    public async Task<Guid?> CreateAsync(DbUserAvatar dbUserAvatar)
    {
      if (dbUserAvatar == null)
      {
        return null;
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

      return dbUserAvatar.AvatarId;
    }

    public async Task<List<Guid>> GetAvatarsByUserId(Guid avatarId)
    {
      return await _provider.UsersAvatars
        .Where(a => a.AvatarId == avatarId)
        .Select(a => a.AvatarId)
        .ToListAsync();
    }

    public async Task<List<DbUserAvatar>> GetAsync(List<Guid> imagesIds)
    {
      if (imagesIds == null || !imagesIds.Any())
      {
        return null;
      }

      return await _provider.UsersAvatars
        .Where(a => imagesIds.Contains(a.AvatarId)).ToListAsync();
    }

    public async Task<bool> UpdateCurrentAvatarAsync(Guid userId, Guid avatarId)
    {
      DbUserAvatar dbUserAvatar = _provider.UsersAvatars.FirstOrDefault(x => x.AvatarId == avatarId);

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

    public async Task<bool> RemoveAsync(List<Guid> imagesIds)
    {
      if (imagesIds == null || !imagesIds.Any())
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
