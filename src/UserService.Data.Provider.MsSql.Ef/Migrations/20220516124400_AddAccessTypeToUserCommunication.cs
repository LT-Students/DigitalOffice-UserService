using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
  [Migration("20220516124400_AddAccessTypeToUserCommunication")]
  [DbContext(typeof(UserServiceDbContext))]
  public class AddAccessTypeToUserCommunication : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<int>(
        name: nameof(DbUserCommunication.VisibleTo),
        table: DbUserCommunication.TableName,
        nullable: false,
        defaultValue: 0);
    }
  }
}
