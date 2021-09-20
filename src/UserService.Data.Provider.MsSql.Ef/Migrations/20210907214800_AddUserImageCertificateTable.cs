using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
  [DbContext(typeof(UserServiceDbContext))]
  [Migration("20210905214200_AddUserImageCertificateTable")]
  class AddUserImageCertificateTable : Migration
  {
    protected override void Up(MigrationBuilder builder)
    {
      builder.CreateTable
        (
          name: DbUserCertificateImage.TableName,
          columns: table => new
          {
            Id = table.Column<Guid>(nullable: false),
            UserCertificateId = table.Column<Guid>(nullable: false),
            ImageId = table.Column<Guid>(nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_UserCertificateImages", uei => uei.Id);
          }
        );
    }

    protected override void Down(MigrationBuilder builder)
    {
      builder.DropTable(DbUserCertificateImage.TableName);
    }
  }
}
