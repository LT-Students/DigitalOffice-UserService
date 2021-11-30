using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.Password
{
  public record ChangePasswordRequest
  {
    public Guid UserId { get; set; }
    public string Password { get; set; }
    public string NewPassword { get; set; }
  }
}
