using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Models.Db
{
    public class DbUser
    {
        public const string TableName = "Users";

        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public int Status { get; set; }
        public Guid? AvatarFileId { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }

        public DateTime CreatedAt { get; set; }
        public DbUserCredentials UserCredentials { get; set; }
        public ICollection<DbUserCertificateFile> CertificatesFiles { get; set; }
        public ICollection<DbUserAchievement> Achievements { get; set; }
        public ICollection<DbConnection> Connections { get; set; }
        public ICollection<DbUserSkills> UserSkills { get; set; }

        public DbUser()
        {
            Connections = new HashSet<DbConnection>();
            Achievements = new HashSet<DbUserAchievement>();
            CertificatesFiles = new HashSet<DbUserCertificateFile>();
            UserSkills = new HashSet<DbUserSkills>();
        }
    }

    public class DbUserConfiguration : IEntityTypeConfiguration<DbUser>
    {
        public void Configure(EntityTypeBuilder<DbUser> builder)
        {
            builder.ToTable(DbUser.TableName);

            builder.HasKey(u => u.Id);
            builder
                .Property(u => u.CreatedAt)
                .HasDefaultValue(new DateTime(2021, 1, 1));

            builder.
                HasKey(p => p.Id);

            builder.Property(u => u.Email);

            builder.Property(u => u.FirstName);

            builder.Property(u => u.LastName);

            builder
                .Property(u => u.MiddleName)
                .IsRequired(false);

            builder
                .Property(u => u.Status);

            builder
                .Property(u => u.AvatarFileId)
                .IsRequired(false);

            builder.Property(u => u.IsActive);

            builder.Property(u => u.IsAdmin);

            builder
                .HasMany(u => u.UserSkills)
                .WithOne(us => us.User)
                .HasForeignKey(us => us.UserId);

            builder
                .HasMany(u => u.Connections)
                .WithOne(conn => conn.User);
        }
    }
}