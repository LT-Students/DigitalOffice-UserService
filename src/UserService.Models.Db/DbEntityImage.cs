using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace LT.DigitalOffice.UserService.Models.Db
{
  public class DbEntityImage
  {
    public const string TableName = "EntitiesImages";

    public Guid Id { get; set; }
    public Guid EntityId { get; set; }
    public Guid ImageId { get; set; }
    public bool IsCurrentAvatar { get; set; }
  }

  public class DbEntityImageConfiguration : IEntityTypeConfiguration<DbEntityImage>
  {
    public void Configure(EntityTypeBuilder<DbEntityImage> builder)
    {
      builder
        .ToTable(DbEntityImage.TableName);

      builder
        .HasKey(c => c.Id);
    }
  }
}
