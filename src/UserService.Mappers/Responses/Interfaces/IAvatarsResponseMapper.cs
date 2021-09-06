using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Avatar;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Mappers.Responses.Interfaces
{
  [AutoInject]
  public interface IAvatarsResponseMapper
  {
    AvatarsResponse Map(List<ImageData> imagesData, Guid? currentAvatar);
  }
}
