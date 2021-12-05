using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Avatar;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Mappers.Db.Interfaces
{
  [AutoInject]
  public interface IDbUserAvatarMapper
  {
    List<DbUserAvatar> Map(List<Guid> imagesIds, CreateAvatarRequest request);
    DbUserAvatar Map(Guid imageId, Guid entityId, bool isCurrentAvatar = false);
  }
}
