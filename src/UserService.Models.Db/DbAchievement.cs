﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Models.Db
{
    public class DbAchievement
    {
        public const string TableName = "Achievements";

        public Guid Id { get; set; }
        public Guid ImageId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<DbUserAchievement> UserAchievements { get; set; }

        public DbAchievement()
        {
            UserAchievements = new HashSet<DbUserAchievement>();
        }
    }

    public class DbAchievementConfiguration : IEntityTypeConfiguration<DbAchievement>
    {
        public void Configure(EntityTypeBuilder<DbAchievement> builder)
        {
            builder
                .ToTable(DbAchievement.TableName);

            builder
                .HasKey(a => a.Id);

            builder
                .Property(a => a.ImageId)
                .IsRequired();

            builder
                .Property(a => a.Name)
                .IsRequired();

            builder
                .HasMany(a => a.UserAchievements)
                .WithOne(ua => ua.Achievement);
        }
    }
}
