using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
  [DbContext(typeof(UserServiceDbContext))]
  [Migration("20211019151500_AddIsCurrentAvatarField")]
  class AddIsCurrentAvatarField : Migration
  {
    protected override void Up(MigrationBuilder builder)
    {
      builder.DropColumn(
        name: "AvatarFileId",
        table: "Users");

      builder.AddColumn<bool>(
        name: "IsCurrentAvatar",
        table: "EntitiesImages",
        defaultValue: "false");
    }
  }
}
