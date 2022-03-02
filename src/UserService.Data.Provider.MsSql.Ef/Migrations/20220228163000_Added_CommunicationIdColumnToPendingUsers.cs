using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
  [DbContext(typeof(UserServiceDbContext))]
  [Migration("20220228163000_Added_CommunicationIdColumnToPendingUsers")]
  internal class Added_CommunicationIdColumnToPendingUsers : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<Guid>(
        name: nameof(DbPendingUser.CommunicationId),
        table: DbPendingUser.TableName,
        defaultValue: Guid.Empty,
        nullable: false);
    }
  }
}
