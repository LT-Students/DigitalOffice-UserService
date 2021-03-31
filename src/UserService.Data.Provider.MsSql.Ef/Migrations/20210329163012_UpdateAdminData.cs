using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.UserCredentials.Admin;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(UserServiceDbContext))]
    [Migration("20210329163012_UpdateAdminData")]
    public partial class UpdateAdminData : Migration
    {
        private const string ADMIN_PASSWORD = "%4fgT1_3ioR";

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: DbUserCredentials.TableName,
                keyColumns: new string[]
                {
                    nameof(DbUserCredentials.UserId)
                },
                keyColumnTypes: new string[]
                {
                    string.Empty
                },
                keyValues: new string[]
                {
                    AdminCredentials.UserId.ToString()
                },
                columns: new string[]
                {
                    nameof(DbUserCredentials.PasswordHash),
                    nameof(DbUserCredentials.Salt)
                },
                columnTypes: new string[]
                {
                    string.Empty,
                    string.Empty
                },
                values: new object[]
                {
                    AdminCredentials.GetPasswordHash(ADMIN_PASSWORD),
                    AdminCredentials.Salt
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
