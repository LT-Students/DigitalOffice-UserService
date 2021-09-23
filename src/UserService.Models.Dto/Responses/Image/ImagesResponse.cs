using LT.DigitalOffice.UserService.Models.Dto.Models;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Models.Dto.Responses.Image
{
  public record ImagesResponse
  {
    public List<ImageInfo> Images { get; set; }
  }
}
