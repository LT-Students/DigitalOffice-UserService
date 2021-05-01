using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(UserServiceDbContext))]
    [Migration("20210429102831_AddUserEducation")]
    class AddUserEducation : Migration
    {
        private void CreateUserEducationTable(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: DbUserEducation.TableName,
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    UniversityName = table.Column<string>(nullable: false),
                    QualificationName = table.Column<string>(nullable: false),
                    FormEducation = table.Column<int>(nullable: false),
                    AdmissiomAt = table.Column<DateTime>(nullable: false),
                    IssueAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Education", x => x.Id);
                });
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            CreateUserEducationTable(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: DbUserEducation.TableName);
        }
    }
}
