using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.UserCredentials.Admin;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(UserServiceDbContext))]
    [Migration("20210329163012_UpdateAdminData")]
    class UpdateAdminData : Migration
    {
        private const string ADMIN_PASSWORD = "%4fgT1_3ioR";

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: DbUserCredentials.TableName,
                keyColumn: nameof(DbUserCredentials.UserId),
                keyValue: AdminCredentials.userId,
                columns: new string[]
                {
                    nameof(DbUserCredentials.PasswordHash),
                    nameof(DbUserCredentials.Salt)
                },
                values: new object[]
                {
                    AdminCredentials.GetPasswordHash(ADMIN_PASSWORD),
                    AdminCredentials.salt
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: DbUserCredentials.TableName,
                keyColumn: nameof(DbUserCredentials.UserId),
                keyValue: AdminCredentials.userId,
                columns: new string[]
                {
                    nameof(DbUserCredentials.PasswordHash),
                    nameof(DbUserCredentials.Salt)
                },
                values: new object[]
                {
                    AdminCredentials.GetPasswordHash(),
                    AdminCredentials.salt
                });
        }
    }
}
