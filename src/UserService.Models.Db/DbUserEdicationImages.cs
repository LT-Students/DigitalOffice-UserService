using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace LT.DigitalOffice.UserService.Models.Db
{
  public class DbUserEducationImages
  {
    public const string TableName = "UserEducationImages";

    public Guid UserEducationId { get; set; }
    public Guid ImageId { get; set; }
 
    public DbUserEducation UserEducation { get; set; }
  }

  public class DbProjectImageConfiguration : IEntityTypeConfiguration<DbUserEducationImages>
  {
    public void Configure(EntityTypeBuilder<DbUserEducationImages> builder)
    {
      builder
        .ToTable(DbUserEducationImages.TableName);

      builder
        .HasKey(uei => new { uei.UserEducationId, uei.ImageId });

      builder
        .HasOne(uei => uei.UserEducation)
        .WithMany(ue => ue.Images);
    }
  }
}
