using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
  [AutoInject]
  public interface IImageRepository
  {
    Task<List<Guid>> CreateAsync(List<DbEntityImage> dbEntityImages);

    List<Guid> GetImagesIds(Guid entityId);

    List<DbEntityImage> Get(List<Guid> imagesIds);

    Task<bool> RemoveAsync(List<Guid> imagesIds);
  }
}
