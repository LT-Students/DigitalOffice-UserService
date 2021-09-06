using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.UserService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Avatar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Mappers.Responses
{
  public class AvatarsResponseMapper : IAvatarsResponseMapper
  {
    public AvatarsResponse Map(List<ImageData> imagesData, Guid? currentAvatar)
    {
      if (imagesData == null)
      {
        return null;
      }

      AvatarsResponse avatarsResponse = new();

      foreach(ImageData imageData in imagesData)
      {
        avatarsResponse.Avatars.Add(
          new AvatarInfo
          {
            Id = imageData.ImageId,
            ParentId = imageData.ParentId,
            Content = imageData.Content,
            Extension = imageData.Extension,
            Name = imageData.Name,
            IsCurrentAvatar = imageData.ImageId == currentAvatar
          });
      }

      return avatarsResponse;
    }
  }
}
