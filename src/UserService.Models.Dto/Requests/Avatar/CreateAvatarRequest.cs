using System;
using System.ComponentModel.DataAnnotations;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.Avatar
{
  public record CreateAvatarRequest
  {
    public Guid? UserId { get; set; }
    public string Name { get; set; }
    [Required]
    public string Content { get; set; }
    [Required]
    public string Extension { get; set; }
    public bool IsCurrentAvatar { get; set; } = false;
  }
}
