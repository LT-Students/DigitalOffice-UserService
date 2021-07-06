using LT.DigitalOffice.UserService.Models.Dto.Enums;

namespace LT.DigitalOffice.UserService.Models.Dto.Models
{
    public record CommunicationInfo
    {
        public CommunicationType Type { get; set; }
        public string Value { get; set; }
    }
}
