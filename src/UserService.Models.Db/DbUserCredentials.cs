using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;

namespace LT.DigitalOffice.UserService.Models.Db
{
    public class DbUserCredentials
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public virtual DbUser User { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        public string Salt { get; set; }
    }

    public class UserCredentialsConfiguration : IEntityTypeConfiguration<DbUserCredentials>
    {
        public void Configure(EntityTypeBuilder<DbUserCredentials> builder)
        {
            builder.HasOne(uc => uc.User)
                .WithOne(u => u.UserCredentials)
                .HasForeignKey<DbUserCredentials>(uc => uc.UserId);
        }
    }
}
