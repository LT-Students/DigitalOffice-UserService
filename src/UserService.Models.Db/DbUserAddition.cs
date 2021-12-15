using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace LT.DigitalOffice.UserService.Models.Db
{
  public class DbUserAddition
  {
    public const string TableName = "UsersAdditions";

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? About { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public double? Latitude { get; set; } 
    public double? Longitude { get; set; } 
    public DateTime? BusinessHoursFromUtc { get; set; } 
    public DateTime? BusinessHoursToUtc { get; set; } 
    public Guid ModifiedBy { get; set; } 
    public DateTime ModifiedAtUtc { get; set; } 
  }

  public class UserAdditionConfiguration : IEntityTypeConfiguration<DbUserAddition>
  {
    public void Configure(EntityTypeBuilder<DbUserAddition> builder)
    {
      builder
        .HasKey(x => x.Id);

      builder
        .ToTable(DbUserAddition.TableName);
    }
  }
}
