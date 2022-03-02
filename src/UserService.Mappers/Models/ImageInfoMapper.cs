using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
  public class ImageInfoMapper : IImageInfoMapper
  {
    public ImageInfo Map(ImageData image)
    {
      if (image is null)
      {
        return default;
      }

      return new ImageInfo
      {
        Id = image.ImageId,
        ParentId = image.ParentId,
        Content = image.Content,
        Extension = image.Extension,
        Name = image.Name
      };
    }
  }
}
