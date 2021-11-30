using System;

namespace LT.DigitalOffice.UserService.Models.Dto
{
  public record ReconstructPasswordRequest
  {
    public Guid UserId { get; set; }
    public string Secret { get; set; }
    public string Login { get; set; }
    public string NewPassword { get; set; }
  }
}