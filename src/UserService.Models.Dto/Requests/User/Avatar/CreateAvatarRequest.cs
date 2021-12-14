using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.User.Avatar
{
  public record CreateAvatarRequest
  {
    public Guid? UserId { get; set; }
    public string Name { get; set; }
    public string Content { get; set; }
    public string Extension { get; set; }
    public bool IsCurrentAvatar { get; set; } = false;
  }
}
