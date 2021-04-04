using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Models
{
    public class ProjectInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
