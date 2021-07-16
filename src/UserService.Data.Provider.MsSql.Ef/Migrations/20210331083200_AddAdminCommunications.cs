using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.UserCredentials.Admin;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(UserServiceDbContext))]
    [Migration("20210331083200_AddAdminCommunications")]
    public class AddAdminCommunications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: DbUserCommunication.TableName,
                columns: new[]
                {
                    nameof(DbUserCommunication.Id),
                    nameof(DbUserCommunication.UserId),
                    nameof(DbUserCommunication.Type),
                    nameof(DbUserCommunication.Value)
                },
                columnTypes: new[]
                {
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                },
                new object[]
                {
                    Guid.NewGuid(),
                    AdminCredentials.UserId,
                    (int)CommunicationType.Email,
                    AdminCredentials.Email
                });

            migrationBuilder.UpdateData(
                table: DbUser.TableName,
                keyColumns: new string[]
                {
                    nameof(DbUser.Id)
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
                    nameof(DbUser.FirstName),
                    nameof(DbUser.LastName)
                },
                columnTypes: new string[]
                {
                    string.Empty,
                    string.Empty
                },
                values: new object[]
                {
                    AdminCredentials.FirstName,
                    AdminCredentials.LastName
                });
        }
    }
}
