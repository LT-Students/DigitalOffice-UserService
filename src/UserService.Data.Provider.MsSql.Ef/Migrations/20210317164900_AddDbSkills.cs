using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(UserServiceDbContext))]
    [Migration("20210317164900_AddDbSkills")]
    class AddDbSkills : Migration
    {
        private void CreateSkillsTable(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: DbSkill.TableName,
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SkillName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                });
        }

        private void CreateUsersSkillsTable(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: DbUserSkills.TableName,
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    SkillId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSkills", x => x.Id);

                    table.ForeignKey(
                        name: "FK_UserSkills_Users",
                        column: x => x.UserId,
                        principalTable: DbUser.TableName,
                        principalColumn: nameof(DbUser.Id),
                        onDelete: ReferentialAction.Cascade
                    );

                    table.ForeignKey(
                        name: "FK_UserSkills_Skills",
                        column: x => x.SkillId,
                        principalTable: DbSkill.TableName,
                        principalColumn: nameof(DbSkill.Id),
                        onDelete: ReferentialAction.Cascade
                    );
                });
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            CreateSkillsTable(migrationBuilder);
            CreateUsersSkillsTable(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(DbUserSkills.TableName);
            migrationBuilder.DropTable(DbSkill.TableName);
        }
    }
}
