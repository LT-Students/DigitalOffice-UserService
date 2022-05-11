using System;
using System.ComponentModel.DataAnnotations;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials
{
  public record ReactivateCredentialsRequest
  {
    public Guid UserId { get; set; }
    [Required]
    public string Password { get; set; }
  }
}
