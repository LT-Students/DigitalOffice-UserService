using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
  [AutoInject]
  public interface IImageRepository
  {
    List<Guid> Create(List<DbEntityImage> dbEntityImages);

    List<Guid> GetImagesIds(Guid entityId);

    List<DbEntityImage> Get(List<Guid> imagesIds);

    bool Remove(List<Guid> imagesIds);
  }
}
