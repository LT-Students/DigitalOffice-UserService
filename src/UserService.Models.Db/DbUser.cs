using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LT.DigitalOffice.UserService.Models.Db
{
    public class DbUser
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Status { get; set; }
        public Guid? AvatarFileId { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }

        public DbUserCredentials UserCredentials { get; set; }
        public ICollection<DbUserCertificateFile> CertificatesFilesIds { get; set; }
        public ICollection<DbUserAchievement> AchievementsIds { get; set; }
        public ICollection<DbUserSkills> Skills { get; set; }

        public DbUser() {
            CertificatesFilesIds = new HashSet<DbUserCertificateFile>();
            AchievementsIds = new HashSet<DbUserAchievement>();
            Skills = new HashSet<DbUserSkills>();
        }
    }

    public class DbUserConfiguration : IEntityTypeConfiguration<DbUser>
    {
        public void Configure(EntityTypeBuilder<DbUser> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Email);
            
            builder.Property(u => u.FirstName);
            
            builder.Property(u => u.LastName);
            
            builder
                .Property(u => u.MiddleName)
                .IsRequired(false);

            builder
                .Property(u => u.Status)
                .IsRequired(false);

            builder
                .Property(u => u.AvatarFileId)
                .IsRequired(false);

            builder.Property(u => u.IsActive);

            builder
                .Property(u => u.IsAdmin)
                .IsRequired(false);

            builder
                .HasMany(u => u.Skills)
                .WithOne(us => us.User)
                .HasForeignKey(us => us.UserId);
        }
    }
}