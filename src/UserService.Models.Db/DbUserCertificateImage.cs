using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace LT.DigitalOffice.UserService.Models.Db
{
  public class DbUserCertificateImage
  {
    public const string TableName = "UserCertificateImages";

    public Guid Id;
    public Guid UserCertificateId { get; set; }
    public Guid ImageId { get; set; }

    public DbUserCertificate UserCertificate { get; set; }
  }

  public class DbUserCertificateImageConfiguration : IEntityTypeConfiguration<DbUserCertificateImage>
  {
    public void Configure(EntityTypeBuilder<DbUserCertificateImage> builder)
    {
      builder
        .ToTable(DbUserCertificateImage.TableName);

      builder
        .HasKey(uei => uei.Id);

      builder
        .HasOne(uei => uei.UserCertificate)
        .WithMany(ue => ue.Images);
    }
  }
}
