using LT.DigitalOffice.UserService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Responses.Achievement
{
  public record AchievementResponse
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ImageConsist Image { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public Guid CreatedBy { get; set; }
  }
}
