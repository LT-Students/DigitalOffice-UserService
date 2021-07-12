using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Models
{
    public record RoleInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
