using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
  [DbContext(typeof(UserServiceDbContext))]
  [Migration("20220204142900_AddIsConfirmendColumnToUsersCommunications")]
  public class AddIsConfirmendColumnToUsersCommunications : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<bool>(
        name: nameof(DbUserCommunication.IsConfirmed),
        table: DbUserCommunication.TableName,
        defaultValue: "false",
        nullable: false);
    }
  }
}
