using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Avatar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers.Db
{
  public class DbUserAvatarMapper : IDbUserAvatarMapper
  {
    public List<DbUserAvatar> Map(List<Guid> imagesIds, CreateAvatarRequest request)
    {
      if (imagesIds == null || !imagesIds.Any() || request == null)
      {
        return null;
      }

      return imagesIds.Select(i => new DbUserAvatar
      {
        Id = Guid.NewGuid(),
        UserId = request.UserId.Value,
        AvatarId = i
      }).ToList();
    }

    public DbUserAvatar Map(Guid avatarId, Guid userId, bool isCurrentAvatar = false)
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
