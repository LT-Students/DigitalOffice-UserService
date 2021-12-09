﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace LT.DigitalOffice.UserService.Models.Db
{
  public class DbUserGender
  {
    public const string TableName = "UsersGenders";

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid GenderId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public DbUser User { get; set; }
    public DbGender Gender { get; set; }
  }

  public class UserGenderConfiguration : IEntityTypeConfiguration<DbUserGender>
  {
    public void Configure(EntityTypeBuilder<DbUserGender> builder)
    {
      builder
        .ToTable(DbUserGender.TableName);

      builder
        .HasKey(ug => ug.Id);
    }
  }
}