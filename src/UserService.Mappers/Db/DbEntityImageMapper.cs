using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Images;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers.Db
{
  public class DbEntityImageMapper : IDbEntityImageMapper
  {
    public List<DbUserAvatar> Map(List<Guid> imagesIds, CreateImageRequest request)
    {
      if (imagesIds == null || !imagesIds.Any() || request == null)
      {
        return null;
      }

      Guid entityId = request.EntityId;

      return imagesIds.Select(x => new DbUserAvatar
      {
        Id = Guid.NewGuid(),
        UserId = entityId,
        ImageId = x
      }).ToList();
    }

    public DbUserAvatar Map(Guid imageId, Guid entityId, bool isCurrentAvatar = false)
    {
      return new DbUserAvatar
      {
        Id = Guid.NewGuid(),
        UserId = entityId,
        ImageId = imageId,
        IsCurrentAvatar = isCurrentAvatar
      };
    }
  }
}
