using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
  [DbContext(typeof(UserServiceDbContext))]
  [Migration("20211130201500_DropEducationAndCertificateTables")]
  public class DropEducationAndCertificateTables : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
        name: "UserCertificates");

      migrationBuilder.DropTable(
        name: "UserEducations");

      migrationBuilder.DropTable(
        name: "EntitiesImages");

      migrationBuilder.CreateTable(
        name: DbUserAvatar.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          UserId = table.Column<Guid>(nullable: false),
          AvatarId = table.Column<Guid>(nullable: false),
          IsCurrentAvatar = table.Column<bool>(nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey($"PK_{DbUserAvatar.TableName}", x => x.Id);
        });
    }
  }
}
