using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace LT.DigitalOffice.UserService.Models.Db
{
    public class DbUserCertificateFile
    {
        public Guid UserId { get; set; }
        public virtual DbUser User { get; set; }
        public Guid CertificateId { get; set; }
    }

    public class UserCertificateFileConfiguration : IEntityTypeConfiguration<DbUserCertificateFile>
    {
        public void Configure(EntityTypeBuilder<DbUserCertificateFile> builder)
        {
            builder.HasKey(pm => new { pm.UserId, pm.CertificateId });
            builder.HasOne(pm => pm.User)
                .WithMany(p => p.CertificatesFilesIds)
                .HasForeignKey(pm => pm.UserId);
        }
    }
}
