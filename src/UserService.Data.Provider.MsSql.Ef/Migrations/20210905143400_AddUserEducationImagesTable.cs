using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
  [DbContext(typeof(UserServiceDbContext))]
  [Migration("20210824232700_AddProjectsImagesTable")]
  class AddUserEducationImagesTable : Migration
  {
    protected override void Up(MigrationBuilder builder)
    {
      builder.CreateTable
        (
          name: DbUserEducationImages.TableName,
          columns: table => new
          {
            UserEducationId = table.Column<Guid>(),
            ImageId = table.Column<Guid>()
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_UserEducationImages", uei => new { uei.UserEducationId, uei.ImageId });
          }
        );
    }

    protected override void Down(MigrationBuilder builder)
    {
      builder.DropTable(DbUserEducationImages.TableName);
    }
  }
}
