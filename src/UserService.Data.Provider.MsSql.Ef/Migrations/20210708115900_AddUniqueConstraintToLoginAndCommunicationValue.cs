using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(UserServiceDbContext))]
    [Migration("20210708115900_AddUniqueConstraintToLoginAndCommunicationValue")]
    public class AddUniqueConstraintToLoginAndCommunicationValue : Migration
    {
        protected override void Up(MigrationBuilder builder)
        {
            builder.AlterColumn<string>(
                name: nameof(DbUserCredentials.Login),
                table: DbUserCredentials.TableName,
                maxLength: 100);

            builder.AddUniqueConstraint(
                name: $"UX_Login_unique",
                table: DbUserCredentials.TableName,
                column: nameof(DbUserCredentials.Login));

            builder.AlterColumn<string>(
                name: nameof(DbUserCommunication.Value),
                table: DbUserCommunication.TableName,
                maxLength: 100);

            builder.AddUniqueConstraint(
                name: $"UX_Value_unique",
                table: DbUserCommunication.TableName,
                column: nameof(DbUserCommunication.Value));
        }
    }
}
