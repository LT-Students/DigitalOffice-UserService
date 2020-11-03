using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(UserServiceDbContext))]
    [Migration("20201031013800_Unique_constraint_user_login")]
    public class AddUniqueConstraintUserLogin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Login",
                table: "UserCredentials",
                maxLength: 16);

            migrationBuilder.AddUniqueConstraint(
                name: "UX_login_unique",
                table: "UserCredentials",
                column: "Login");
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                 name: "UX_login_unique",
                 table: "UserCredentials");

            migrationBuilder.AlterColumn<string>(
                name: "Login",
                table: "UserCredentials",
                maxLength: 16);
        }
    }
}