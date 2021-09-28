using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Models
{
  public record AchievementInfo
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ImageConsist Image { get; set; }
    public DateTime CreatedAtUtc { get; set; }
  }
}
