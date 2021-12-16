using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Models
{
  public record UserSkillInfo
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
  }
}
