using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace LT.DigitalOffice.UserService.Models.Db
{
    public class DbUserSkill
    {
        public const string TableName = "UserSkills";

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid SkillId { get; set; }
        public DbUser User { get; set; }
        public DbSkill Skill { get; set; }
    }

    public class DbUserSkillsConfiguration : IEntityTypeConfiguration<DbUserSkill>
    {
        public void Configure(EntityTypeBuilder<DbUserSkill> builder)
        {
            builder
                .ToTable(DbUserSkill.TableName);

            builder
                .HasKey(us => us.Id);

            builder
                .Property(us => us.UserId)
                .IsRequired();

            builder
                .Property(us => us.SkillId)
                .IsRequired();

            builder
                .HasOne(us => us.User)
                .WithMany(u => u.Skills)
                .HasForeignKey(us => us.UserId);

            builder
                .HasOne(us => us.Skill)
                .WithMany(s => s.UserSkills)
                .HasForeignKey(us => us.SkillId);
        }
    }
}
