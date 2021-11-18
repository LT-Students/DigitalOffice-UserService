using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Communication;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Models.Dto
{
    public record CreateUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public UserGender Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public AddImageRequest AvatarImage { get; set; }
        public UserStatus Status { get; set; }
        public bool? IsAdmin { get; set; }
        public DateTime? StartWorkingAt { get; set; }
        public double Rate { get; set; }
        public Guid? PositionId { get; set; }
        public Guid? OfficeId { get; set; }
        public Guid? RoleId { get; set; }
        public Guid? DepartmentId { get; set; }
        public string Password { get; set; }
        public IEnumerable<CreateCommunicationRequest> Communications { get; set; }
    }
}