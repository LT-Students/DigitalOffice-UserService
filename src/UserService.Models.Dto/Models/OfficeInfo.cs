using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Models
{
    public record OfficeInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
    }
}
