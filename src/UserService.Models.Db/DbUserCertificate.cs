using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace LT.DigitalOffice.UserService.Models.Db
{
    public class DbUserCertificate
    {
        public const string TableName = "UserCertificates";

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ImageId { get; set; }
        public int EducationType { get; set; }
        public string Name { get; set; }
        public string SchoolName { get; set; }
        public bool IsActive { get; set; }
        public DateTime ReceivedAt { get; set; }
        public DbUser User { get; set; }
    }

    public class DbUserCertificateConfiguration : IEntityTypeConfiguration<DbUserCertificate>
    {
        public void Configure(EntityTypeBuilder<DbUserCertificate> builder)
        {
            builder
                .ToTable(DbUserCertificate.TableName);

            builder
                .HasKey(c => c.Id);

            builder
                .HasOne(pm => pm.User)
                .WithMany(p => p.Certificates)
                .HasForeignKey(pm => pm.UserId);
        }
    }
}
