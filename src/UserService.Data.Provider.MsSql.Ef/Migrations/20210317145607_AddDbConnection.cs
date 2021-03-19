using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(UserServiceDbContext))]
    [Migration("20210317145607_AddDbConnection")]
    public class AddUserConnection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable("UserConnections", table => new
            {
                Id = table.Column<Guid>(nullable: false),
                Value = table.Column<string>(nullable: false),
                Type = table.Column<int>(nullable: false),
                UserId = table.Column<Guid>(nullable: false)
            },
             constraints: table => 
             {
                 table.PrimaryKey("PK_UserConnection", a => a.Id);
                 table.ForeignKey(
                         name: "FK_UserConnections_Users",
                         column: x => x.UserId,
                         principalTable: "Users",
                         principalColumn: "Id",
                         onDelete: ReferentialAction.Cascade);
             });
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserConnections");

           
        }
    }
}
