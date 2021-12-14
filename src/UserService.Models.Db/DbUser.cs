using LT.DigitalOffice.Kernel.BrokerSupport.Attributes.ParseEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Models.Db
{
  [ParseEntity]
  public class DbUser
  {
    public const string TableName = "Users";

    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public int Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string City { get; set; }
    public int Status { get; set; }
    public bool IsAdmin { get; set; }
    public string About { get; set; }
    public bool IsActive { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }

    [IgnoreParse]
    public DbUserCredentials Credentials { get; set; }
    [IgnoreParse]
    public DbUserLocation Location { get; set; }
    public DbUserGender UserGender { get; set; }
    [IgnoreParse]
    public ICollection<DbUserAvatar> Avatars { get; set; }
    [IgnoreParse]
    public ICollection<DbUserCommunication> Communications { get; set; }
    [IgnoreParse]
    public ICollection<DbUserAchievement> Achievements { get; set; }
    [IgnoreParse]
    public ICollection<DbUserSkill> Skills { get; set; }

    public DbUser()
    {
      Avatars = new HashSet<DbUserAvatar>();
      Communications = new HashSet<DbUserCommunication>();
      Achievements = new HashSet<DbUserAchievement>();
      Skills = new HashSet<DbUserSkill>();
    }
  }

  public class DbUserConfiguration : IEntityTypeConfiguration<DbUser>
  {
    public void Configure(EntityTypeBuilder<DbUser> builder)
    {
      builder.
        ToTable(DbUser.TableName);

      builder.
        HasKey(p => p.Id);

      builder
        .Property(p => p.FirstName)
        .IsRequired();

      builder
        .Property(p => p.LastName)
        .IsRequired();

      builder
        .HasMany(u => u.Avatars)
        .WithOne(ua => ua.User);

      builder
        .HasOne(u => u.Credentials)
        .WithOne(uc => uc.User);

      builder
        .HasMany(u => u.Communications)
        .WithOne(uc => uc.User);

      builder
        .HasMany(u => u.Achievements)
        .WithOne(ua => ua.User);

      builder
        .HasMany(u => u.Skills)
        .WithOne(us => us.User);

      builder
        .HasOne(u => u.Location)
        .WithOne(ul => ul.User);

      builder
        .HasOne(u => u.UserGender)
        .WithOne(ug => ug.User);
    }
  }
}