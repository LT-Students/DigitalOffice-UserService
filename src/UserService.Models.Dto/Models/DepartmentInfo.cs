using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Models
{
    public class DepartmentInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime StartWorkingAt { get; set; }
    }
}
