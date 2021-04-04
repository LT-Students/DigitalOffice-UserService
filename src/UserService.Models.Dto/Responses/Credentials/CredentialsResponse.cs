using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Responses.Credentials
{
    public class CredentialsResponse
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
    }
}
