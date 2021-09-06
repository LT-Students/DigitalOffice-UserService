using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
  [AutoInject]
  public interface IAvatarRepository
  {
    List<Guid> Create(List<DbUserAvatar> dbUserAvatars);

    List<DbUserAvatar> Get(Guid userId);

    List<DbUserAvatar> Get(List<Guid> imagesIds);

    bool Remove(List<Guid> imageIds);
  }
}
