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

        public ICollection<DbUserSkills> Users { get; set; }

        public DbSkill()
        {
            Users = new HashSet<DbUserSkills>();
        }
    }

    public class DbSkillConfiguration : IEntityTypeConfiguration<DbSkill>
    {
        public void Configure(EntityTypeBuilder<DbSkill> builder)
        {
            builder.ToTable(DbSkill.TableName);

            builder.HasKey(s => s.Id);

            builder.Property(s => s.SkillName);

            builder
                .HasMany(s => s.Users)
                .WithOne(us => us.Skill)
                .HasForeignKey(us => us.SkillId);
        }
    }
}
