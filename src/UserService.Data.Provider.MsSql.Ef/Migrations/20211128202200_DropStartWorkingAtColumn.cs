using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
  [DbContext(typeof(UserServiceDbContext))]
  [Migration("20211128202200_DropStartWorkingAtColumn")]
  public class DropStartWorkingAtColumn : Migration
  {
    protected override void Up(MigrationBuilder builder)
    {
      builder.DropColumn(
        name: "StartWorkingAt",
        table: DbUser.TableName);
    }

    protected override void Down(MigrationBuilder builder)
    {
      builder.AddColumn<DateTime>(
        name: "StartWorkingAt",
        table: DbUser.TableName,
        nullable: true);
    }
  }
}
