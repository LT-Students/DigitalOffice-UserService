using LT.DigitalOffice.UserService.Models.Dto.Enums;
using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.User.Communication
{
    public record CreateCommunicationRequest
    {
        public Guid UserId { get; set; }
        public CommunicationType Type { get; set; }
        public string Value { get; set; }
    }
}
