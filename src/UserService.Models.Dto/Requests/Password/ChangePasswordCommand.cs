using System.ComponentModel.DataAnnotations;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.Password
{
  public record ChangePasswordRequest
  {
    [Required]
    public string Password { get; set; }
    [Required]
    public string NewPassword { get; set; }
  }
}
