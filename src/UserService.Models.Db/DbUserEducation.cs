using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

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

        public DbUser User { get; set; }
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
                .IsRequired();

            builder
                .Property(e => e.QualificationName)
                .IsRequired();

            builder
                .HasOne(e => e.User)
                .WithMany(u => u.Educations)
                .HasForeignKey(e => e.UserId);
        }
    }
}
