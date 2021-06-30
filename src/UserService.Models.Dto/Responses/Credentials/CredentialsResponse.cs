using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Responses.Credentials
{
    public record CredentialsResponse
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
    }
}
