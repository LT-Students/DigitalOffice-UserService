using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(UserServiceDbContext))]
    [Migration("20210317145607_AddDbConnection")]
    public class AddUserConnection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable("Connections", table => new
            {
                Id = table.Column<Guid>(nullable: false),
                Value = table.Column<string>(nullable: false),
                Type = table.Column<int>(nullable: false),
                UserId = table.Column<Guid>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Connection", a => a.Id);
                table.ForeignKey(
                        name: "FK_Connections_Users",
                        column: x => x.UserId,
                        principalTable: DbUser.TableName,
                        principalColumn: nameof(DbUser.Id),
                        onDelete: ReferentialAction.Cascade);
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("Connections");
        }
    }
}
