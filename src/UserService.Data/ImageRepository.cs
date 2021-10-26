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

    public async Task<Guid?> CreateAsync(DbEntityImage dbEntityImage)
    {
      if (dbEntityImage == null)
      {
        return null;
      }

      _provider.EntitiesImages.Add(dbEntityImage);
      await _provider.SaveAsync();

      return dbEntityImage.ImageId;
    }

    public async Task<List<Guid>> GetImagesIdsByEntityIdAsync(Guid entityId)
    {
      return await _provider.EntitiesImages.Where(x => x.EntityId == entityId).Select(x => x.ImageId).ToListAsync();
    }

    public async Task<List<DbEntityImage>> GetAsync(List<Guid> imagesIds)
    {
      if (imagesIds == null || !imagesIds.Any())
      {
        return null;
      }

      return await _provider.EntitiesImages.Where(x => imagesIds.Contains(x.ImageId)).ToListAsync();
    }

    public async Task<Guid?> UpdateAvatarAsync(Guid userId, Guid imageId)
    {
      List<DbEntityImage> dbEntityImages = await GetAsync(new List<Guid> { imageId });
      DbEntityImage image = dbEntityImages?.FirstOrDefault();

      if (image == null || image.EntityId != userId)
      {
        return null;
      }

      List<DbEntityImage> userImages = await _provider.EntitiesImages.Where(x => userId == x.EntityId && x.IsCurrentAvatar).ToListAsync();

      userImages.Add(image);

      _provider.EntitiesImages.UpdateRange(userImages);

      userImages?.ForEach(x => x.IsCurrentAvatar = false);
      image.IsCurrentAvatar = true;
      await _provider.SaveAsync();

      return image.ImageId;
    }

    public async Task<Guid?> UpdateAvatarAsync(DbEntityImage avatarImage)
    {
      if (avatarImage == null)
      {
        return null;
      }

      List<DbEntityImage> userImages = await _provider.EntitiesImages.Where(x => avatarImage.EntityId == x.EntityId && x.IsCurrentAvatar).ToListAsync();

      _provider.EntitiesImages.UpdateRange(userImages);
      userImages?.ForEach(x => x.IsCurrentAvatar = false);

      avatarImage.IsCurrentAvatar = true;
      _provider.EntitiesImages.Add(avatarImage);

      await _provider.SaveAsync();

      return avatarImage.ImageId;
    }

    public async Task<DbEntityImage> GetAvatarAsync(Guid userId)
    {
      return await _provider.EntitiesImages.Where(x => x.EntityId == userId && x.IsCurrentAvatar).FirstOrDefaultAsync();
    }

    public async Task<List<DbEntityImage>> GetAvatarsAsync(List<Guid> usersIds)
    {
      if (usersIds == null || !usersIds.Any())
      {
        return null;
      }

      return await _provider.EntitiesImages.Where(x => (usersIds.Contains(x.EntityId) && x.IsCurrentAvatar)).ToListAsync();
    }

    public async Task<bool> RemoveAsync(List<Guid> imagesIds)
    {
      if (imagesIds == null || !imagesIds.Any())
      {
        return false;
      }

      List<DbEntityImage> removeUsersAvatars = await GetAsync(imagesIds);

      _provider.EntitiesImages.RemoveRange(removeUsersAvatars);
      await _provider.SaveAsync();

      return true;
    }
  }
}
