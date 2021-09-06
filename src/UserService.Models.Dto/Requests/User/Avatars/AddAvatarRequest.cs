using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.User.Avatars
{
  public record AddAvatarRequest
  {
    public Guid UserId { get; set; }
    public List<AddImageRequest> Images { get; set; }
  }
}
