using LT.DigitalOffice.UserService.Models.Dto.Enums;
using System;

namespace LT.DigitalOffice.UserService.Models.Dto.Models
{
    public class UserInfo
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public UserStatus Status { get; set; }
        public bool IsAdmin { get; set; }
        public string About { get; set; }
        public string StartWorkingAt { get; set; }
    }
}
