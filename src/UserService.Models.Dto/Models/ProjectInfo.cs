using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Models
{
    public record ProjectInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }
}
