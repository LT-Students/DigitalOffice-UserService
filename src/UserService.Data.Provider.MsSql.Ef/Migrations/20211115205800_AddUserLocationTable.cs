using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using LT.DigitalOffice.UserService.Models.Db;
using System;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
  [DbContext(typeof(UserServiceDbContext))]
  [Migration("20211115205800_AddUserLocationTable")]
  class AddUserLocationTable : Migration
  {
    protected override void Up(MigrationBuilder builder)
    {
      builder.CreateTable(
        name: DbUserLocation.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          UserId = table.Column<Guid>(nullable: false),
          Latitude = table.Column<double>(nullable: true),
          Longitude = table.Column<double>(nullable: true),
          BusinessHoursFromUtc = table.Column<DateTime>(nullable: true),
          BusinessHoursToUtc = table.Column<DateTime>(nullable: true),
          CreatedBy = table.Column<Guid>(nullable: false),
          CreatedAtUtc = table.Column<DateTime>(nullable: false),
          ModifiedBy = table.Column<Guid>(nullable: true),
          ModifiedAtUtc = table.Column<DateTime>(nullable: true)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_UserLocationTable", x => x.Id);
        });
    }
  }
}
