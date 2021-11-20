using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
  [DbContext(typeof(UserServiceDbContext))]
  [Migration("20211118130200_AddGenderTableAndUserGenderTable")]
  public class AddGenderTableAndUserGenderTable : Migration
  {
    public void CreateUserGenderTable(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: DbUserGender.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          UserId = table.Column<Guid>(nullable: false),
          GenderId = table.Column<Guid>(nullable: false),
          CreatedAtUtc = table.Column<DateTime>(nullable: false),
          ModifiedAtUtc = table.Column<DateTime>(nullable: true)
        },
        constraints: table =>
        {
          table.PrimaryKey("PR_UsersGenders", x => x.Id);
        });
    }

    public void CreateGenderTable(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: DbGender.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          Name = table.Column<string>(nullable: false),
          CreatedBy = table.Column<Guid>(nullable: false),
          CreatedAtUtc = table.Column<DateTime>(nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PR_Genders", x => x.Id);
        });
    }

    protected override void Up(MigrationBuilder migrationBuilder)
    {
      CreateUserGenderTable(migrationBuilder);
      CreateGenderTable(migrationBuilder);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
        name: DbGender.TableName);
      migrationBuilder.DropTable(
        name: DbUserGender.TableName);
    }
  }
}

