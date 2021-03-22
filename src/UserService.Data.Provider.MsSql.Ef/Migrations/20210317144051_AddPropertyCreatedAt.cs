using System;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(UserServiceDbContext))]
    [Migration("20210317144051_AddPropertyCreatedAt")]
    public class AddPropertyCreatedAt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: nameof(DbUser.CreatedAt),
                table: DbUser.TableName,
                nullable: false,
                defaultValue: new DateTime(2021, 1, 1)
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: nameof(DbUser.CreatedAt),
                table: DbUser.TableName
            );
        }
    }
}