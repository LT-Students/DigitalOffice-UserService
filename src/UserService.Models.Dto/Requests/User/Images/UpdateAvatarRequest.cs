using Microsoft.AspNetCore.Mvc;
using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.User.Images
{
  public class UpdateAvatarRequest
  {
    public Guid UserId { get; set; }
    public Guid? ImageId { get; set; } = null;
    public string Name { get; set; } = null;
    public string Content { get; set; } = null;
    public string Extension { get; set; } = null;
  }
}
