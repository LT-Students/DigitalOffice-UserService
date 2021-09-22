using LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.MessageService.Data.Provider.MsSql.Ef.Migrations
{
  [Migration("20210922152800_UpdateDbAchievement")]
  [DbContext(typeof(UserServiceDbContext))]
  public class UpdateDbAchievement : Migration
  {
    protected override void Up(MigrationBuilder builder)
    {
      builder.AddColumn<string>(
        name: nameof(DbAchievement.ImageContent),
        table: DbAchievement.TableName,
        nullable: true);

      builder.AddColumn<string>(
        name: nameof(DbAchievement.ImageContent),
        table: DbAchievement.TableName,
        nullable: true);

      builder.DropColumn(
        name: "ImageId",
        table: DbAchievement.TableName);
    }
    protected override void Down(MigrationBuilder builder)
    {
      builder.DropColumn(
        name: nameof(DbAchievement.ImageContent),
        table: DbAchievement.TableName);

      builder.DropColumn(
        name: nameof(DbAchievement.ImageContent),
        table: DbAchievement.TableName);

      builder.AddColumn<Guid>(
        name: "ImageId",
        table: DbAchievement.TableName);
    }
  }
}
