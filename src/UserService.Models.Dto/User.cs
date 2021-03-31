using LT.DigitalOffice.UserService.Models.Dto.Enums;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Models.Dto
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public UserStatus Status { get; set; }
        public Guid? AvatarFileId { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<Communications> Communications { get; set; }
        public IEnumerable<Guid> CertificatesIds { get; set; }
        public IEnumerable<Achievement> AchievementsIds { get; set; }
     }
}