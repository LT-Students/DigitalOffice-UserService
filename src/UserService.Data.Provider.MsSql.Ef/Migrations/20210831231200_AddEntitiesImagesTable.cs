using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
  [DbContext(typeof(UserServiceDbContext))]
  [Migration("20210831231200_AddEntitiesImagesTable")]
  class AddEntitiesImagesTable : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: "EntitiesImages",
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          EntityId = table.Column<Guid>(nullable: false),
          ImageId = table.Column<Guid>(nullable: false),
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_EntitiesImages", x => x.Id);
        });
    }
  }
}
