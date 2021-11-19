using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
  [Migration("20211118220400_RemoveCityRenameUsersImagesTable")]
  [DbContext(typeof(UserServiceDbContext))]
  public class RemoveCityRenameUsersImagesTable : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.RenameIndex(
        name: "PK_EntitiesImages",
        newName: $"PK_{DbUserAvatar.TableName}",
        table: "EntitiesImages");

      migrationBuilder.RenameColumn(
        name: "EntityId",
        table: "EntitiesImages",
        newName: nameof(DbUserAvatar.UserId));

      migrationBuilder.RenameTable(
        name: "EntitiesImages",
        newName: DbUserAvatar.TableName);

      migrationBuilder.DropColumn(
        name: "City",
        table: "Users");
    }
  }
}
