using System;

namespace LT.DigitalOffice.UserService.Models.Dto
{
    public class ChangePasswordRequest
    {
        public Guid UserId { get; set; }
        public Guid Secret { get; set; }
        public string Login { get; set; }
        public string NewPassword { get; set; }
    }
}