using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
  [DbContext(typeof(UserServiceDbContext))]
  [Migration("20210905214200_AddUserEducationImagesTable")]
  class AddUserEducationImagesTable : Migration
  {
    protected override void Up(MigrationBuilder builder)
    {
      builder.CreateTable
        (
          name: DbUserEducationImage.TableName,
          columns: table => new
          {
            Id = table.Column<Guid>(nullable: false),
            UserEducationId = table.Column<Guid>(nullable: false),
            ImageId = table.Column<Guid>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_UserEducationImages", uei => uei.Id);
          }
        );
    }

    protected override void Down(MigrationBuilder builder)
    {
      builder.DropTable(DbUserEducationImage.TableName);
    }
  }
}
