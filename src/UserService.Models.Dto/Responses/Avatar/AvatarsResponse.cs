using LT.DigitalOffice.UserService.Models.Dto.Models;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Models.Dto.Responses.Avatar
{
  public record AvatarsResponse
  {
    public List<AvatarInfo> Avatars { get; set; } = new();
  }
}
