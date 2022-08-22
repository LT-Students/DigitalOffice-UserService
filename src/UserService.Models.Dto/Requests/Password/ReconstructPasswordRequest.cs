using System;
using System.ComponentModel.DataAnnotations;

namespace LT.DigitalOffice.UserService.Models.Dto
{
  public record ReconstructPasswordRequest
  {
    public Guid UserId { get; set; }
    [Required]
    public string Secret { get; set; }
    [Required]
    public string Login { get; set; }
    [Required]
    public string NewPassword { get; set; }
  }
}