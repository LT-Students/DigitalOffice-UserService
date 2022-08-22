using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using System;

namespace LT.DigitalOffice.UserService.Mappers.Db
{
  public class DbUserAvatarMapper : IDbUserAvatarMapper
  {
    public DbUserAvatar Map(
      Guid avatarId,
      Guid userId,
      bool isCurrentAvatar = false)
    {
      return new DbUserAvatar
      {
        Id = Guid.NewGuid(),
        UserId = userId,
        AvatarId = avatarId,
        IsCurrentAvatar = isCurrentAvatar
      };
    }
  }
}
