using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;

namespace LT.DigitalOffice.UserService.Models.Db
{
    public class DbUserAchievement
    {
        public Guid UserId { get; set; }
        public virtual DbUser User { get; set; }
        public Guid AchievementId { get; set; }
        public virtual DbAchievement Achievement { get; set; }
        [Required]
        public DateTime Time { get; set; }
    }

    public class UserAchievementConfiguration : IEntityTypeConfiguration<DbUserAchievement>
    {
        public void Configure(EntityTypeBuilder<DbUserAchievement> builder)
        {
            builder.HasKey(pm => new { pm.UserId, pm.AchievementId });
            builder.HasOne(pm => pm.User)
                .WithMany(p => p.AchievementsIds)
                .HasForeignKey(pm => pm.UserId);
        }
    }
}
