using System;
using System.ComponentModel.DataAnnotations;

namespace LT.DigitalOffice.UserService.Models.Dto
{
  public record ReconstructPasswordRequest
  //password and secred must be receiven from body! receiving from query is not safe
  {
    public Guid UserId { get; set; }

    [Required]
    public string Secret { get; set; }

    [Required]
    public string NewPassword { get; set; }
  }
}