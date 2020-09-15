﻿using LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using System;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(UserServiceDbContext))]
    partial class UserServiceDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("LT.DigitalOffice.UserService.Models.Db.DbAchievement", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<string>("Message")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<Guid>("PictureFileId")
                    .HasColumnType("uniqueidentifier");

                b.HasKey("Id");

                b.ToTable("Achievements");
            });

            modelBuilder.Entity("LT.DigitalOffice.UserService.Models.Db.DbUser", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<Guid?>("AvatarFileId")
                    .HasColumnType("uniqueidentifier");

                b.Property<string>("Email")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("FirstName")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<bool>("IsActive")
                    .HasColumnType("bit");

                b.Property<bool>("IsAdmin")
                    .HasColumnType("bit");

                b.Property<string>("LastName")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("MiddleName")
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("PasswordHash")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("Status")
                    .HasColumnType("nvarchar(max)");

                b.HasKey("Id");

                b.ToTable("Users");
            });

            modelBuilder.Entity("LT.DigitalOffice.UserService.Models.Db.DbUserAchievement", b =>
            {
                b.Property<Guid>("UserId")
                    .HasColumnType("uniqueidentifier");

                b.Property<Guid>("AchievementId")
                    .HasColumnType("uniqueidentifier");

                b.Property<DateTime>("Time")
                    .HasColumnType("datetime2");

                b.HasKey("UserId", "AchievementId");

                b.HasIndex("AchievementId");

                b.ToTable("DbUserAchievement");
            });

            modelBuilder.Entity("LT.DigitalOffice.UserService.Models.Db.DbUserCertificateFile", b =>
            {
                b.Property<Guid>("UserId")
                    .HasColumnType("uniqueidentifier");

                b.Property<Guid>("CertificateId")
                    .HasColumnType("uniqueidentifier");

                b.HasKey("UserId", "CertificateId");

                b.ToTable("DbUserCertificateFile");
            });

            modelBuilder.Entity("LT.DigitalOffice.UserService.Models.Db.DbUserAchievement", b =>
            {
                b.HasOne("LT.DigitalOffice.UserService.Models.Db.DbAchievement", "Achievement")
                    .WithMany()
                    .HasForeignKey("AchievementId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("LT.DigitalOffice.UserService.Models.Db.DbUser", "User")
                    .WithMany("AchievementsIds")
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("LT.DigitalOffice.UserService.Models.Db.DbUserCertificateFile", b =>
            {
                b.HasOne("LT.DigitalOffice.UserService.Models.Db.DbUser", "User")
                    .WithMany("CertificatesFilesIds")
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });
#pragma warning restore 612, 618
        }
    }
}
