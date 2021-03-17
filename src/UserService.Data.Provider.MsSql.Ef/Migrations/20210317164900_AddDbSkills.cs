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
                    table.PrimaryKey("PK_Skill", x => x.Id);
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
                    table.PrimaryKey("PK_UserSkill", x => x.Id);

                    table.ForeignKey(
                        name: "FK_UserSkills_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);

                    table.ForeignKey(
                        name: "FK_UserSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: DbSkill.TableName,
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });
        }

        private void AddFKUsersToUserSkills(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserSkills_UserId",
                table: "Users",
                column: "Id",
                principalTable: DbUserSkills.TableName,
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade
                );
        }

        private void AddFKSkillsToUserSkill(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_Skill_UserSkills_SkillId",
                table: DbSkill.TableName,
                column: "Id",
                principalTable: DbUserSkills.TableName,
                principalColumn: "SkillId",
                onDelete: ReferentialAction.Cascade
                );
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            CreateSkillsTable(migrationBuilder);
            CreateUsersSkillsTable(migrationBuilder);
            //AddFKUsersToUserSkills(migrationBuilder);
            //AddFKSkillsToUserSkill(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(DbSkill.TableName);
            migrationBuilder.DropTable(DbUserSkills.TableName);
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserSkills_UserId",
                table: "Users"
                );
            migrationBuilder.DropForeignKey(
                name: "FK_Skill_UserSkills_SkillId",
                table: DbSkill.TableName
                );
        }
    }
}
