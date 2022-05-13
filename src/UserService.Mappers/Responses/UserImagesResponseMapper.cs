using LT.DigitalOffice.UserService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Image;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Mappers.Responses
{
  public class UserImagesResponseMapper : IUserImagesResponseMapper
  {
    public UserImagesResponse Map(Guid userId, List<ImageInfo> images)
    {
      return images is null
        ? default
        : new UserImagesResponse
        {
          UserId = userId,
          Images = images
        };
    }
  }
}
