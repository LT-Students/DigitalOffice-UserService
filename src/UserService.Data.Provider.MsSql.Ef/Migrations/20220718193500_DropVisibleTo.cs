using System;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
  [DbContext(typeof(UserServiceDbContext))]
  [Migration("20220718193500_DropVisibleTo")]
  class DropVisibleTo : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropColumn(
        name: "VisibleTo",
        table: DbUserCommunication.TableName);
    }
    
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AddColumn<int>(
        name: "VisibleTo",
        table: DbUserCommunication.TableName,
        nullable: false,
        defaultValue: false);
    }
  }
}