using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{

    [DbContext(typeof(UserServiceDbContext))]
    [Migration("20210316204900_ChangeUserStatus")]
    public class ChangeUserStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Users",
                nullable: false,
                defaultValue: 0);
        }
    }
}
