using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Models
{
  public class AvatarInfo
  {
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string Content { get; set; }
    public string Extension { get; set; }
    public string Name { get; set; }
    public bool IsCurrentAvatar { get; set; }
  }
}
