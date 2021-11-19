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
    Task<Guid?> CreateAsync(DbUserAvatar dbEntityImage);

    Task<List<Guid>> GetImagesIdsByEntityIdAsync(Guid entityId);

    Task<List<DbUserAvatar>> GetAsync(List<Guid> imagesIds);

    Task<List<DbUserAvatar>> GetAvatarsAsync(List<Guid> usersIds);

    Task<DbUserAvatar> GetAvatarAsync(Guid userId);

    Task<Guid?> UpdateAvatarAsync(Guid userId, Guid imageId);

    Task<Guid?> UpdateAvatarAsync(DbUserAvatar avatarImage);

    Task<bool> RemoveAsync(List<Guid> imagesIds);
  }
}
