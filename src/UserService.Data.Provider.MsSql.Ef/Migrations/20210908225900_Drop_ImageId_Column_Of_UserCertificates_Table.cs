using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
  [DbContext(typeof(UserServiceDbContext))]
  [Migration("20210908225900_Drop_ImageId_Column_Of_UserCertificates_Table")]
  class Drop_ImageId_Column_Of_UserCertificates_Table : Migration
  {
    protected override void Up(MigrationBuilder builder)
    {
      builder.DropColumn(name: "ImageId", table: DbUserCertificate.TableName);
    }

    protected override void Down(MigrationBuilder builder)
    {
      builder.AddColumn<Guid>(name: "ImageId", table: DbUserCertificate.TableName, nullable: false);
    }
  }
}
