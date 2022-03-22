using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Models
{
  public record CompanyUserInfo
  {
    public CompanyInfo Company { get; set; }
    public double? Rate { get; set; }
    public DateTime? StartWorkingAt { get; set; }
  }
}
