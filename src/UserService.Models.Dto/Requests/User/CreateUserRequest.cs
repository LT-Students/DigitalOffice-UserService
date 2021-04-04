using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Models.Dto
{
    public class CreateUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public UserStatus Status { get; set; }
        public string Password { get; set; }
        public bool? IsAdmin { get; set; }
        public string StartWorkingAt { get; set; }
        public string AvatarImage { get; set; }
        public Guid? PositionId { get; set; }
        public Guid? DepartmentId { get; set; }
        public IEnumerable<CommunicationInfo> Communications { get; set; }
    }
}