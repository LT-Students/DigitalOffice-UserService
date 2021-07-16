using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Models.Db
{
    public class DbSkill
    {
        public const string TableName = "Skills";

        public Guid Id { get; set; }
        public string SkillName { get; set; }
        public ICollection<DbUserSkill> UserSkills { get; set; }

        public DbSkill()
        {
            UserSkills = new HashSet<DbUserSkill>();
        }
    }

    public class DbSkillConfiguration : IEntityTypeConfiguration<DbSkill>
    {
        public void Configure(EntityTypeBuilder<DbSkill> builder)
        {
            builder
                .ToTable(DbSkill.TableName);

            builder
                .HasKey(s => s.Id);

            builder
                .Property(s => s.SkillName)
                .IsRequired();

            builder
                .HasMany(s => s.UserSkills)
                .WithOne(us => us.Skill);
        }
    }
}
