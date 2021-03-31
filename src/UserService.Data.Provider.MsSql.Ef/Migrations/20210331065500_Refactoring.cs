using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(UserServiceDbContext))]
    [Migration("20210331065500_Refactoring")]
    public class ChangeConnectionsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Connections",
                newName: DbUserCommunication.TableName);

            migrationBuilder.DropColumn("Email", DbUser.TableName);
        }
    }
}
