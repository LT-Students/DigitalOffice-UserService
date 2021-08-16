﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace LT.DigitalOffice.UserService.Models.Db
{
    public class DbUserAchievement
    {
        public const string TableName = "UserAchievements";

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid AchievementId { get; set; }
        public DateTime ReceivedAt { get; set; }
        public bool IsActive { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedAtUtc { get; set; }
        public DbUser User { get; set; }
        public DbAchievement Achievement { get; set; }
    }

    public class DbUserAchievementConfiguration : IEntityTypeConfiguration<DbUserAchievement>
    {
        public void Configure(EntityTypeBuilder<DbUserAchievement> builder)
        {
            builder
                .ToTable(DbUserAchievement.TableName);

            builder
                .HasKey(ua => ua.Id);

            builder
                .HasOne(ua => ua.Achievement)
                .WithMany(a => a.UserAchievements);

            builder
                .HasOne(ua => ua.User)
                .WithMany(u => u.Achievements);
        }
    }
}
