using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
  [AutoInject]
  public interface IUserAvatarRepository
  {
    Task CreateAsync(DbUserAvatar dbEntityImage);

    Task<List<Guid>> GetAvatarsByUserId(Guid entityId);

    Task<List<DbUserAvatar>> GetAsync(List<Guid> imagesIds);

    Task<bool> UpdateCurrentAvatarAsync(Guid userId, Guid imageId);

    Task<bool> RemoveAsync(List<Guid> imagesIds);
  }
}
