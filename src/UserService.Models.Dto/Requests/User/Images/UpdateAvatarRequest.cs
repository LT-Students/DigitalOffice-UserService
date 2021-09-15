using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.User.Images
{
  public record UpdateAvatarRequest
  {
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Content { get; set; }
    public string Extension { get; set; }
  }
}
