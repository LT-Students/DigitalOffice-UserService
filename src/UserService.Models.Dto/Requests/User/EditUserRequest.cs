using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.User
{
    public class EditUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string AvatarImage { get; set; }
        public UserStatus Status { get; set; }
        public IEnumerable<CertificateInfo> Certificates { get; set; }
    }
}