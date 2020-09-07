using System;
using System.Collections.Generic;

namespace UserService.Models.Dto
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public Guid? AvatarId { get; set; }
        public bool IsAdmin { get; set; }
        public IEnumerable<Guid> CertificatesIds { get; set; }
        public IEnumerable<Achievement> Achievements { get; set; }
    }
}
