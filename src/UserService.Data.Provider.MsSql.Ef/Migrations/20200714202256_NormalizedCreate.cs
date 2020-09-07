using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace UserService.Data.Provider.MsSql.Ef.Migrations
{
    public partial class NormalizedCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Obsolete",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "CEO",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "PictureId",
                table: "Achievements");

            migrationBuilder.DropColumn(
                name: "Time",
                table: "Achievements");

            migrationBuilder.AddColumn<Guid>(
                name: "AvatarFileId",
                table: "Users",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "CEOUserId",
                table: "Companies",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Companies",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "PictureFileId",
                table: "Achievements",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "UserAchievement",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    AchievementId = table.Column<Guid>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAchievement", x => new { x.UserId, x.AchievementId });
                    table.ForeignKey(
                        name: "FK_UserAchievement_Achievements_AchievementId",
                        column: x => x.AchievementId,
                        principalTable: "Achievements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAchievement_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCertificateFile",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    CertificateId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCertificateFile", x => new { x.UserId, x.CertificateId });
                    table.ForeignKey(
                        name: "FK_UserCertificateFile_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPosition",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    PositionId = table.Column<Guid>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPosition", x => new { x.UserId, x.PositionId });
                    table.ForeignKey(
                        name: "FK_UserPosition_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPosition_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAchievement_AchievementId",
                table: "UserAchievement",
                column: "AchievementId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPosition_PositionId",
                table: "UserPosition",
                column: "PositionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAchievement");

            migrationBuilder.DropTable(
                name: "UserCertificateFile");

            migrationBuilder.DropTable(
                name: "UserPosition");

            migrationBuilder.DropColumn(
                name: "AvatarFileId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CEOUserId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "PictureFileId",
                table: "Achievements");

            migrationBuilder.AddColumn<Guid>(
                name: "AvatarId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "Obsolete",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "Positions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "Positions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CEO",
                table: "Companies",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PictureId",
                table: "Achievements",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "Time",
                table: "Achievements",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
