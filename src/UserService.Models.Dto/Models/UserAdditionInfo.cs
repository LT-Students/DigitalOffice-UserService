using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Models
{
  public record UserAdditionInfo
  {
    public GenderInfo Gender { get; set; }
    public string About { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime? BusinessHoursFromUtc { get; set; }
    public DateTime? BusinessHoursToUtc { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
  }
}
