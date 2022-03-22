using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Image;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Mappers.Responses.Interfaces
{
  [AutoInject]
  public interface IImagesResponseMapper
  {
    public ImagesResponse Map(List<ImageInfo> images);
  }
}
