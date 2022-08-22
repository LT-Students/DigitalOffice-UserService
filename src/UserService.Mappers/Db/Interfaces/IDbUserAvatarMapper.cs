using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using System;

namespace LT.DigitalOffice.UserService.Mappers.Db.Interfaces
{
  [AutoInject]
  public interface IDbUserAvatarMapper
  {
    DbUserAvatar Map(
      Guid imageId,
      Guid entityId,
      bool isCurrentAvatar = false);
  }
}
