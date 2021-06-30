using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials
{
    public record CreateCredentialsRequest
    {
        public Guid UserId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
