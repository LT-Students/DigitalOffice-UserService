using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace LT.DigitalOffice.UserService.Models.Db
{
  public class DbUserLocation
  {
    public const string TableName = "UsersLocations";

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime? BusinessHoursFromUtc { get; set; }
    public DateTime? BusinessHoursToUtc { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }

    public DbUser User { get; set; }
  }

  public class DbUserLocationConfiguration : IEntityTypeConfiguration<DbUserLocation>
  {
    public void Configure(EntityTypeBuilder<DbUserLocation> builder)
    {
      builder
        .ToTable(DbUserLocation.TableName);

      builder
        .HasKey(x => x.Id);

      builder
        .HasOne(l => l.User)
        .WithOne(ul => ul.Location);
    }
  }
}
