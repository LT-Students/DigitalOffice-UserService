using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
  [DbContext(typeof(UserServiceDbContext))]
  [Migration("20211213223200_DropUsersLocationsTableAndSomeColumnsFromUsersTable")]
  public class DropUsersLocationsTableAndSomeColumnsFromUsersTable : Migration
  {
    protected override void Up(MigrationBuilder builder)
    {
      builder.DropTable(
        name: "UsersLocations");

      builder.DropColumn(
        name: "DateOfBirth",
        table: DbUser.TableName);

      builder.DropColumn(
        name: "City",
        table: DbUser.TableName);

      builder.DropColumn(
        name: "About",
        table: DbUser.TableName);
    }
  }
}
