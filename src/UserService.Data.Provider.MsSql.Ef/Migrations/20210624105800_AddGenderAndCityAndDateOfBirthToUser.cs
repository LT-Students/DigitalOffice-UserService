using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(UserServiceDbContext))]
    [Migration("20210624105800_AddGenderAndCityAndDateOfBirthToUser")]
    public class AddGenderAndCityAndDateOfBirthToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: nameof(DbUser.City),
                table: DbUser.TableName,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: nameof(DbUser.Gender),
                table: DbUser.TableName,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: nameof(DbUser.DateOfBirth),
                table: DbUser.TableName,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: nameof(DbUser.StartWorkingAt),
                table: DbUser.TableName,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: nameof(DbUser.City),
                table: DbUser.TableName);

            migrationBuilder.DropColumn(
                name: nameof(DbUser.Gender),
                table: DbUser.TableName);

            migrationBuilder.DropColumn(
                name: nameof(DbUser.DateOfBirth),
                table: DbUser.TableName);
        }
    }
}
