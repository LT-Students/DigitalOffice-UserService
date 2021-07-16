using LT.DigitalOffice.UserService.Models.Dto.Enums;
using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.User
{
    public class EditUserRequest
    {
        public Guid DepartmentId { get; set; }
        public Guid PositionId { get; set; }
        public Guid RoleId { get; set; }
        public Guid OfficeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string About { get; set; }
        public UserGender Gender { get; set; }
        public string DateOfBirth { get; set; }
        public string City { get; set; }
        public AddImageRequest AvatarImage { get; set; }
        public UserStatus Status { get; set; }
        public string StartWorkingAt { get; set; }
        public double Rate { get; set; }
        public bool IsActive { get; set; }
    }
}