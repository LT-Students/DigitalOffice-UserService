using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Models
{
    public class PositionInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime ReceivedAt { get; set; }
    }
}
