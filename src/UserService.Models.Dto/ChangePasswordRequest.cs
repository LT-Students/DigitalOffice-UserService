namespace LT.DigitalOffice.UserService.Models.Dto
{
    public class ChangePasswordRequest
    {
        public string Login { get; set; }
        public string NewPassword { get; set; }
    }
}
