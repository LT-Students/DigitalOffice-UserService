﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LT.DigitalOffice.UserService.Models.Db
{
    public class DbUser
    {
        public const string TableName = "Users";

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
        public ICollection<DbUserCertificateFile> CertificatesFiles { get; set; }
        public ICollection<DbUserAchievement> Achievements { get; set; }
        public ICollection<DbConnection> Connections { get; set; }

        public DbUser()
        {
            Connections = new HashSet<DbConnection>();
            Achievements = new HashSet<DbUserAchievement>();
            CertificatesFiles = new HashSet<DbUserCertificateFile>();
        }
    }

    public class DbUserConfiguration : IEntityTypeConfiguration<DbUser>
    {
        public void Configure(EntityTypeBuilder<DbUser> builder)
        {
            builder.
                ToTable(DbUser.TableName);

            builder.
                HasKey(p => p.Id);

            builder
                .Property(p => p.Email)
                .IsRequired();

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
                .HasMany(u => u.Connections)
                .WithOne(conn => conn.User);
        }
    }
}