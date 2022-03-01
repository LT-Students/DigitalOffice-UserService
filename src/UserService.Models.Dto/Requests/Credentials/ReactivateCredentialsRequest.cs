using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials
{
  public record ReactivateCredentialsRequest
  {
    public Guid UserId { get; set; }
    public string Password { get; set; }
  }
}
