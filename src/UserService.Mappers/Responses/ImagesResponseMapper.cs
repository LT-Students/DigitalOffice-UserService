using LT.DigitalOffice.UserService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Image;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Mappers.Responses
{
  public class ImagesResponseMapper : IImagesResponseMapper
  {
    public ImagesResponse Map(List<ImageInfo> images)
    {
      if (images is null)
      {
        return null;
      }

      return new ImagesResponse
      {
        Images = images
      };
    }
  }
}
