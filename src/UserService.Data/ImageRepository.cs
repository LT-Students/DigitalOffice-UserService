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
  public class ImageRepository : IImageRepository
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

      _provider.UsersAvatars.Add(dbUserAvatar);
      await _provider.SaveAsync();

      return dbUserAvatar.ImageId;
    }

    public async Task<List<Guid>> GetImagesIdsByEntityIdAsync(Guid userId)
    {
      return await _provider.UsersAvatars.Where(x => x.UserId == userId).Select(x => x.ImageId).ToListAsync();
    }

    public async Task<List<DbUserAvatar>> GetAsync(List<Guid> imagesIds)
    {
      if (imagesIds == null || !imagesIds.Any())
      {
        return null;
      }

      return await _provider.UsersAvatars.Where(x => imagesIds.Contains(x.ImageId)).ToListAsync();
    }

    public async Task<Guid?> UpdateAvatarAsync(Guid userId, Guid imageId)
    {
      DbUserAvatar image = _provider.UsersAvatars.FirstOrDefault(x => x.ImageId == imageId);

      if (image == null || image.UserId != userId)
      {
        return null;
      }

      List<DbUserAvatar> userImages = await _provider.UsersAvatars.Where(x => userId == x.UserId && x.IsCurrentAvatar).ToListAsync();

      userImages.Add(image);

      _provider.UsersAvatars.UpdateRange(userImages);

      userImages?.ForEach(x => x.IsCurrentAvatar = false);
      image.IsCurrentAvatar = true;
      await _provider.SaveAsync();

      return image.ImageId;
    }

    public async Task<Guid?> UpdateAvatarAsync(DbUserAvatar dbUserAvatar)
    {
      if (dbUserAvatar == null)
      {
        return null;
      }

      List<DbUserAvatar> userImages = await _provider.UsersAvatars.Where(x => dbUserAvatar.UserId == x.UserId && x.IsCurrentAvatar).ToListAsync();

      _provider.UsersAvatars.UpdateRange(userImages);
      userImages?.ForEach(x => x.IsCurrentAvatar = false);

      dbUserAvatar.IsCurrentAvatar = true;
      _provider.UsersAvatars.Add(dbUserAvatar);

      await _provider.SaveAsync();

      return dbUserAvatar.ImageId;
    }

    public async Task<DbUserAvatar> GetAvatarAsync(Guid userId)
    {
      return await _provider.UsersAvatars.Where(x => x.UserId == userId && x.IsCurrentAvatar).FirstOrDefaultAsync();
    }

    public async Task<List<DbUserAvatar>> GetAvatarsAsync(List<Guid> usersIds)
    {
      if (usersIds == null || !usersIds.Any())
      {
        return null;
      }

      return await _provider.UsersAvatars.Where(ua => (usersIds.Contains(ua.UserId) && ua.IsCurrentAvatar)).ToListAsync();
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
