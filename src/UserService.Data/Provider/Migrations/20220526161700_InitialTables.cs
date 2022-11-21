using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
  [DbContext(typeof(UserServiceDbContext))]
  [Migration("20220526161700_InitialTables")]
  class InitialTables : Migration
  {
    protected override void Up(MigrationBuilder builder)
    {
      builder.CreateTable(
        name: DbGender.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          Name = table.Column<string>(nullable: false, maxLength: 40),
          CreatedBy = table.Column<Guid>(nullable: false),
          CreatedAtUtc = table.Column<DateTime>(nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey($"PK_{DbGender.TableName}", x => x.Id);
          table.UniqueConstraint($"UC_{DbGender.TableName}_Name", x => x.Name);
        });

      builder.CreateTable(
        name: DbUser.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          FirstName = table.Column<string>(nullable: false),
          LastName = table.Column<string>(nullable: false),
          MiddleName = table.Column<string>(nullable: true),
          IsAdmin = table.Column<bool>(nullable: false),
          IsActive = table.Column<bool>(nullable: false),
          CreatedBy = table.Column<Guid>(nullable: false),
          PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
            .Annotation("SqlServer:IsTemporal", true)
            .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
            .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
          PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false)
            .Annotation("SqlServer:IsTemporal", true)
            .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
            .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
        },
        constraints: table =>
        {
          table.PrimaryKey($"PK_{DbUser.TableName}", x => x.Id);
        })
        .Annotation("SqlServer:IsTemporal", true)
        .Annotation("SqlServer:TemporalHistoryTableName", $"{DbUser.TableName}History")
        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

      builder.CreateTable(
        name: DbUserAddition.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          UserId = table.Column<Guid>(nullable: false),
          GenderId = table.Column<Guid>(nullable: true),
          About = table.Column<string>(nullable: true),
          DateOfBirth = table.Column<DateTime>(nullable: true),
          BusinessHoursFromUtc = table.Column<DateTime>(nullable: true),
          BusinessHoursToUtc = table.Column<DateTime>(nullable: true),
          Latitude = table.Column<double>(nullable: true),
          Longitude = table.Column<double>(nullable: true),
          ModifiedBy = table.Column<Guid>(nullable: false),
          ModifiedAtUtc = table.Column<DateTime>(nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey($"PK_{DbUserAddition.TableName}", x => x.Id);
        });

      builder.CreateTable(
        name: DbUserAvatar.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          UserId = table.Column<Guid>(nullable: false),
          AvatarId = table.Column<Guid>(nullable: false),
          IsCurrentAvatar = table.Column<bool>(nullable: false),
        },
        constraints: table =>
        {
          table.PrimaryKey($"PK_{DbUserAvatar.TableName}", x => x.Id);
        });

      builder.CreateTable(
        name: DbUserCommunication.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          UserId = table.Column<Guid>(nullable: false),
          Type = table.Column<int>(nullable: false),
          Value = table.Column<string>(nullable: false, maxLength: 100),
          IsConfirmed = table.Column<bool>(nullable: false),
          CreatedBy = table.Column<Guid>(nullable: false),
          CreatedAtUtc = table.Column<DateTime>(nullable: false),
          ModifiedBy = table.Column<Guid>(nullable: true),
          ModifiedAtUtc = table.Column<DateTime>(nullable: true)
        },
        constraints: table =>
        {
          table.PrimaryKey($"PK_{DbUserCommunication.TableName}", x => x.Id);
          table.UniqueConstraint($"UC_{DbUserCommunication.TableName}_Value", x => x.Value);
        });

      builder.CreateTable(
        name: DbUserCredentials.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          UserId = table.Column<Guid>(nullable: false),
          Login = table.Column<string>(nullable: false, maxLength: 100),
          PasswordHash = table.Column<string>(nullable: false),
          Salt = table.Column<string>(nullable: false),
          IsActive = table.Column<bool>(nullable: false),
          CreatedAtUtc = table.Column<DateTime>(nullable: false),
          ModifiedBy = table.Column<Guid>(nullable: true),
          ModifiedAtUtc = table.Column<DateTime>(nullable: true)
        },
        constraints: table =>
        {
          table.PrimaryKey($"PK_{DbUserCredentials.TableName}", x => x.Id);
          table.UniqueConstraint($"UC_{DbUserCredentials.TableName}_Login", x => x.Login);
        });

      builder.CreateTable(
          name: DbPendingUser.TableName,
          columns: table => new
          {
            UserId = table.Column<Guid>(nullable: false),
            CommunicationId = table.Column<Guid>(nullable: false),
            Password = table.Column<string>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey($"PK_{DbPendingUser.TableName}", x => x.UserId);
          });
    }
  }
}
