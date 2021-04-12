using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(UserServiceDbContext))]
    [Migration("20210401215200_Refactoring")]
    public class _20210401215200_Refactoring : Migration
    {
        private void DropTables(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("UserCertificateFile");
            migrationBuilder.DropTable("UserAchievement");
            migrationBuilder.DropTable("Achievements");
        }

        private void ChangeUserTable(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: nameof(DbUser.StartWorkingAt),
                table: DbUser.TableName,
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: nameof(DbUser.About),
                table: DbUser.TableName,
                type: "nvarchar(150)",
                nullable: true);
        }

        private void CreateAchievementsTable(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: DbAchievement.TableName,
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ImageId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ReceivedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.Id);
                });
        }

        private void CreateUserAchievementsTable(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: DbUserAchievement.TableName,
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    AchievementId = table.Column<Guid>(nullable: false),
                    ReceivedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAchievements", x => x.Id);
                });
        }

        private static void CreateUserCertificatesTable(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: DbUserCertificate.TableName,
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    ImageId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    SchoolName = table.Column<string>(nullable: false),
                    ReceivedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCertificates", x => x.Id);
                });
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            DropTables(migrationBuilder);

            ChangeUserTable(migrationBuilder);

            CreateAchievementsTable(migrationBuilder);

            CreateUserAchievementsTable(migrationBuilder);

            CreateUserCertificatesTable(migrationBuilder);
        }
    }
}
