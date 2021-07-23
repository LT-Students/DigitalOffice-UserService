using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Models
{
    public record UserInfo
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string City { get; set; }
        public string Status { get; set; }
        public bool IsAdmin { get; set; }
        public string About { get; set; }
        public DateTime? StartWorkingAt { get; set; }
        public double Rate { get; set; }
        public bool IsActive { get; set; }

        public DepartmentInfo Department { get; set; }
        public PositionInfo Position { get; set; }
        public ImageInfo Avatar { get; set; }
        public RoleInfo Role { get; set; }
        public OfficeInfo Office { get; set; }
    }
}
