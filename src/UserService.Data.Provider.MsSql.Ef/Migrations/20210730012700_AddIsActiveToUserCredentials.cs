using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(UserServiceDbContext))]
    [Migration("20210730012700_AddIsActiveToUserCredentials")]
    public class AddIsActiveToUserCredentials : Migration
    {
        protected override void Up(MigrationBuilder builder)
        {
            builder.AddColumn<bool>(
                name: nameof(DbUserCredentials.IsActive),
                table: DbUserCredentials.TableName,
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder builder)
        {
            builder.DropColumn(
                name: nameof(DbUserCredentials.IsActive),
                table: DbUserCredentials.TableName);
        }
    }
}
