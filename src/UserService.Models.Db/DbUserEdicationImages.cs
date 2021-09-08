using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace LT.DigitalOffice.UserService.Models.Db
{
  public class DbUserEducationImage
  {
    public const string TableName = "UserEducationImages";

    public Guid Id;
    public Guid UserEducationId { get; set; }
    public Guid ImageId { get; set; }
 
    public DbUserEducation UserEducation { get; set; }
  }

  public class DbUserEducationImageConfiguration : IEntityTypeConfiguration<DbUserEducationImage>
  {
    public void Configure(EntityTypeBuilder<DbUserEducationImage> builder)
    {
      builder
        .ToTable(DbUserEducationImage.TableName);

      builder
        .HasKey(uei => uei.Id);

      builder
        .HasOne(uei => uei.UserEducation)
        .WithMany(ue => ue.Images);
    }
  }
}
