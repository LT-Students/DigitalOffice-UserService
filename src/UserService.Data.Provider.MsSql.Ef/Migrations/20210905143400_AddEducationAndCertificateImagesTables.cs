using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
  [DbContext(typeof(UserServiceDbContext))]
  [Migration("20210905143400_AddEducationAndCertificateImagesTables")]
  class AddEducationAndCertificateImagesTables : Migration
  {
    private void CreateUserEducationImagesTable(MigrationBuilder builder)
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

    private void CreateUserCertificateImagesTable(MigrationBuilder builder)
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

    protected override void Up(MigrationBuilder builder)
    {
      CreateUserEducationImagesTable(builder);
      CreateUserCertificateImagesTable(builder);

      builder.DropColumn(name: "ImageId", table: DbUserCertificate.TableName);
    }

    protected override void Down(MigrationBuilder builder)
    {
      builder.DropTable(DbUserEducationImage.TableName);
      builder.DropTable(DbUserCertificateImage.TableName);

      builder.AddColumn<Guid>(name: "ImageId", table: DbUserCertificate.TableName, nullable: false);
    }
  }
}
