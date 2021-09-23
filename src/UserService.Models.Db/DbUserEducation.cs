using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Models.Db
{
  public class DbUserEducation
  {
    public const string TableName = "UserEducations";

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UniversityName { get; set; }
    public string QualificationName { get; set; }
    public int FormEducation { get; set; }
    public DateTime AdmissionAt { get; set; }
    public DateTime? IssueAt { get; set; }
    public bool IsActive { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }

    public DbUser User { get; set; }
    public ICollection<DbUserEducationImage> Images { get; set; }

    public DbUserEducation()
    {
      Images = new List<DbUserEducationImage>();
    }
  }

  public class DbUserEducationConfiguration : IEntityTypeConfiguration<DbUserEducation>
  {
    public void Configure(EntityTypeBuilder<DbUserEducation> builder)
    {
      builder
        .ToTable(DbUserEducation.TableName);

      builder
        .HasKey(e => e.Id);

      builder
        .Property(e => e.UniversityName)
        .IsRequired()
        .HasMaxLength(100);

      builder
        .Property(e => e.QualificationName)
        .IsRequired()
        .HasMaxLength(100);

      builder
        .HasOne(e => e.User)
        .WithMany(u => u.Educations);

      builder
        .HasMany(ue => ue.Images)
        .WithOne(uei => uei.UserEducation);
    }
  }
}
