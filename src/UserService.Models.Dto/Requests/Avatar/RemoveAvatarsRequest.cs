using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.Avatar
{
  public record RemoveAvatarsRequest
  {
    public Guid UserId { get; set; }
    public List<Guid> AvatarsIds { get; set; }
  }
}
