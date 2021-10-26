using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Images;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Mappers.Db.Interfaces
{
  [AutoInject]
  public interface IDbEntityImageMapper
  {
    List<DbEntityImage> Map(List<Guid> imagesIds, CreateImageRequest request);
    DbEntityImage Map(Guid imageId, Guid entityId, bool isCurrentAvatar = false);
  }
}
