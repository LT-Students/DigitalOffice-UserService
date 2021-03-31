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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public int Status { get; set; }
        public Guid? AvatarFileId { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime CreatedAt { get; set; }
        public DbUserCredentials Credentials { get; set; }
        public ICollection<DbUserCertificateFile> CertificatesFiles { get; set; }
        public ICollection<DbUserAchievement> Achievements { get; set; }
        public ICollection<DbUserCommunication> Communications { get; set; }
        public ICollection<DbUserSkills> UserSkills { get; set; }

        public DbUser()
        {
            Communications = new HashSet<DbUserCommunication>();
            Achievements = new HashSet<DbUserAchievement>();
            CertificatesFiles = new HashSet<DbUserCertificateFile>();
            UserSkills = new HashSet<DbUserSkills>();
        }
    }

    public class DbUserConfiguration : IEntityTypeConfiguration<DbUser>
    {
        public void Configure(EntityTypeBuilder<DbUser> builder)
        {
            builder.
                ToTable(DbUser.TableName);

            builder
                .Property(u => u.CreatedAt)
                .HasDefaultValue(new DateTime(2021, 1, 1));

            builder.
                HasKey(p => p.Id);

            builder
                .Property(p => p.FirstName)
                .IsRequired();

            builder
                .Property(p => p.LastName)
                .IsRequired();

            builder
                .Property(p => p.IsActive)
                .IsRequired();

            builder
                .HasOne(u => u.Credentials)
                .WithOne(uc => uc.User);

            builder
                .HasMany(u => u.Communications)
                .WithOne(conn => conn.User);

            builder
                .HasMany(u => u.UserSkills)
                .WithOne(us => us.User);
        }
    }
}