using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
  [AutoInject]
  public interface IImageRepository
  {
    Task<List<Guid>> CreateAsync(List<DbEntityImage> dbEntityImages);

    Task<List<Guid>> GetImagesIdsByEntityIdAsync(Guid entityId);

    Task<List<DbEntityImage>> GetAsync(List<Guid> imagesIds);

    Task<List<DbEntityImage>> GetAvatarsAsync(List<Guid> usersIds);

    Task<DbEntityImage> GetAvatarAsync(Guid userId);

    Task<Guid?> UpdateAvatarAsync(Guid userId, Guid imageId);

    Task<Guid?> UpdateAvatarAsync(DbEntityImage avatarImage);

    Task<bool> RemoveAsync(List<Guid> imagesIds);
  }
}
