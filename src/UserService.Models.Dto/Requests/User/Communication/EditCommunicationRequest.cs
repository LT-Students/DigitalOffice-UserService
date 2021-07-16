using LT.DigitalOffice.UserService.Models.Dto.Enums;
using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.User.Communication
{
    public record EditCommunicationRequest
    {
        public CommunicationType Type { get; set; }
        public string Value { get; set; }
    }
}
