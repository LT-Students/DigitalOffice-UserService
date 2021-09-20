using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.UserService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Image;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers.Responses
{
  public class ImagesResponseMapper : IImagesResponseMapper
  {
    public ImagesResponse Map(List<ImageData> imagesData)
    {
      if (imagesData == null)
      {
        return null;
      }

      ImagesResponse imagesResponse = new();
      imagesResponse.Images = new();

      imagesResponse.Images.AddRange(imagesData.Select(
          x => new ImageInfo
          {
            Id = x.ImageId,
            ParentId = x.ParentId,
            Content = x.Content,
            Extension = x.Extension,
            Name = x.Name
          }).ToList());

      return imagesResponse;
    }
  }
}
