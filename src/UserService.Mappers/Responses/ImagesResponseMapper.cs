using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.UserService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Image;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Mappers.Responses
{
  public class ImagesResponseMapper : IImagesResponseMapper
  {
    public ImagesResponse Map(List<ImageData> imagesData)
    {
      ImagesResponse imagesResponse = new();
      imagesResponse.Images = new();

      if (imagesData == null)
      {
        imagesResponse = null;
      }
      else
      {
        foreach (ImageData imageData in imagesData)
        {
          imagesResponse.Images.Add(
            new ImageInfo
            {
              Id = imageData.ImageId,
              ParentId = imageData.ParentId,
              Content = imageData.Content,
              Extension = imageData.Extension,
              Name = imageData.Name
            });
        }
      }
      return imagesResponse;
    }
  }
}
