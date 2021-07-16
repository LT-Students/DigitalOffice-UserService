using LT.DigitalOffice.UserService.UserCredentials.Admin;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(UserServiceDbContext))]
    [Migration("20201011111059_ReworkDbModels")]
    public class ReworkDbModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "UserCredentials");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Login",
                table: "UserCredentials",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[]
                {
                    "Id",
                    "Email",
                    "FirstName",
                    "LastName",
                    "IsActive",
                    "IsAdmin"
                },
                columnTypes: new[]
                {
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    "bit",
                    "bit"
                },
                values: new object[]
                {
                    AdminCredentials.UserId,
                    AdminCredentials.Email,
                    AdminCredentials.FirstName,
                    AdminCredentials.LastName,
                    true,
                    true
                });

            migrationBuilder.InsertData(
                table: "UserCredentials",
                columns: new[]
                {
                    "Id",
                    "UserId",
                    "Login",
                    "PasswordHash",
                    "Salt"
                },
                columnTypes: new[]
                {
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty
                },
                values: new object[]
                {
                    Guid.NewGuid(),
                    AdminCredentials.UserId,
                    AdminCredentials.Login,
                    AdminCredentials.GetPasswordHash(),
                    AdminCredentials.Salt
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserCredentials",
                keyColumn: "Id",
                keyValue: new Guid("AD4E3116-55FD-4769-B80D-A6C7E6436296"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: AdminCredentials.UserId);

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Login",
                table: "UserCredentials");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "UserCredentials",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
