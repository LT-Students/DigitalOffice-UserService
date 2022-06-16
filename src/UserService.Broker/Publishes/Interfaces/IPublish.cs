using LT.DigitalOffice.Kernel.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Publishes.Interfaces
{
  [AutoInject]
  public interface IPublish
  {
    Task RemoveImagesAsync(List<Guid> imagesIds);
  }
}
