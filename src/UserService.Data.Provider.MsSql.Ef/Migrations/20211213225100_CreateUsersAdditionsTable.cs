using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
  [DbContext(typeof(UserServiceDbContext))]
  [Migration("20211213225100_CreateUsersAdditionsTable")]
  public class CreateUsersAdditionsTable : Migration
  {
    protected override void Up(MigrationBuilder builder)
    {
      builder.CreateTable(
        name: DbUserAddition.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          UserId = table.Column<Guid>(nullable: false),
          DateOfBirth = table.Column<DateTime>(nullable: true),
          About = table.Column<string>(nullable: true),
          Latitude = table.Column<double>(nullable: true),
          Longitude = table.Column<double>(nullable: true),
          BusinessHoursFromUtc = table.Column<DateTime>(nullable: true),
          BusinessHoursToUtc = table.Column<DateTime>(nullable: true),
          ModifiedBy = table.Column<Guid>(nullable: false),
          ModifiedAtUtc = table.Column<DateTime>(nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_UsersAdditions", x => x.Id);
        });
    }
  }
}
