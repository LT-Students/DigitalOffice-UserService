using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace LT.DigitalOffice.UserService.Models.Db
{
    public class DbUserSkills
    {
        public const string TableName = "UserSkills";
        
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid SkillId { get; set; }

        public DbUser User { get; set; }
        public DbSkill Skill { get; set; }
    }

    public class DbUserSkillsConfiguration : IEntityTypeConfiguration<DbUserSkills>
    {
        public void Configure(EntityTypeBuilder<DbUserSkills> builder)
        {
            builder.ToTable(DbUserSkills.TableName);

            builder.HasKey(us => us.Id);

            builder.Property(us => us.UserId);

            builder.Property(us => us.SkillId);

            builder
                .HasOne(us => us.User)
                .WithMany(u => u.Skills)
                .HasForeignKey(us => us.UserId);

            builder
                .HasOne(us => us.Skill)
                .WithMany(s => s.Users)
                .HasForeignKey(us => us.SkillId);
        }
    }
}
