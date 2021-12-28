using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Avatar;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Mappers.Models.Interfaces
{
  [AutoInject]
  public interface ICreateImageDataMapper
  {
    List<CreateImageData> Map(List<CreateAvatarRequest> request);
  }
}
