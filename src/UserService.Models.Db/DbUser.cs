using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LT.DigitalOffice.UserService.Models.Db
{
    public class DbUser
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Status { get; set; }
        public Guid? AvatarFileId { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public DbUserCredentials UserCredentials { get; set; }
        public ICollection<DbUserCertificateFile> CertificatesFilesIds { get; set; }
        public ICollection<DbUserAchievement> AchievementsIds { get; set; }
        public ICollection<DbConnection> Connections { get; set; }
    }
}