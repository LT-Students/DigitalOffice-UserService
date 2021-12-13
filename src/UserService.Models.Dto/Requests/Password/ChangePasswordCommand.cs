namespace LT.DigitalOffice.UserService.Models.Dto.Requests.Password
{
  public record ChangePasswordRequest
  {
    public string Password { get; set; }
    public string NewPassword { get; set; }
  }
}
