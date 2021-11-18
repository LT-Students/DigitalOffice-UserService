using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Models.Db
{
  public class DbGender
  {
    public const string TableName = "Genders";

    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public ICollection<DbUserGender> UserGenders { get; set; }
  }

  public class GenderConfiguretion : IEntityTypeConfiguration<DbGender>
  {
    public void Configure(EntityTypeBuilder<DbGender> builder)
    {
      builder
          .ToTable(DbGender.TableName);

      builder
          .HasKey(g => g.Id);

      builder
          .Property(g => g.Name)
          .IsRequired();

      builder
          .HasMany(g => g.UserGenders)
          .WithOne(ug => ug.Gender);
    }
  }
}
