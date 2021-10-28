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
    public List<DbEntityImage> Map(List<Guid> imagesIds, CreateImageRequest request)
    {
      if (imagesIds == null || !imagesIds.Any() || request == null)
      {
        return null;
      }

      Guid entityId = request.EntityId;

      return imagesIds.Select(x => new DbEntityImage
      {
        Id = Guid.NewGuid(),
        EntityId = entityId,
        ImageId = x
      }).ToList();
    }

    public DbEntityImage Map(Guid imageId, Guid entityId, bool isCurrentAvatar = false)
    {
      return new DbEntityImage
      {
        Id = Guid.NewGuid(),
        EntityId = entityId,
        ImageId = imageId,
        IsCurrentAvatar = isCurrentAvatar
      };
    }
  }
}
