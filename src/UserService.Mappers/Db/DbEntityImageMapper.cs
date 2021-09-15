using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers.Db
{
  public class DbEntityImageMapper : IDbEntityImageMapper
  {
    public List<DbEntityImage> Map(List<Guid> imagesIds, Guid entityId)
    {
      List<DbEntityImage> result = new();

      if (imagesIds == null || !imagesIds.Any())
      {
        result = null;
      }
      else
      {
        foreach (Guid imageId in imagesIds)
        {
          result.Add(new DbEntityImage
          {
            Id = Guid.NewGuid(),
            EntityId = entityId,
            ImageId = imageId
          });
        }
      }
      return result;
    }
  }
}
