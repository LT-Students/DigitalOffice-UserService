using LT.DigitalOffice.UserService.Models.Db;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Models.Dto
{
    public class UserRequest
    {
        public Guid? Id { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Status { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public List<UserConnection> Connection { get; set; }
        public Guid? AvatarFileId { get; set; }
    }
}