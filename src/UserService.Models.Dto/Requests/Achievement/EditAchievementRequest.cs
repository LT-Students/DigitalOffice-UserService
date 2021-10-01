using LT.DigitalOffice.UserService.Models.Dto.Models;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.Achievement
{
  public record EditAchievementRequest
  {
    public string Name { get; set; }
    public string Description { get; set; }
    public ImageConsist Image { get; set; }
  }
}
