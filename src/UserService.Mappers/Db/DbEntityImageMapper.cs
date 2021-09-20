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
      if (imagesIds == null || !imagesIds.Any())
      {
        return null;
      }

      return imagesIds.Select(x => new DbEntityImage
      {
        Id = Guid.NewGuid(),
        EntityId = entityId,
        ImageId = x
      }).ToList();
    }
  }
}
