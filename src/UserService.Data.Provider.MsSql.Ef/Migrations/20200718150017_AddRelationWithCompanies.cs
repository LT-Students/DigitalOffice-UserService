using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace UserService.Data.Provider.MsSql.Ef.Migrations
{
    public partial class AddRelationWithCompanies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "Positions",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_CompanyId",
                table: "Positions",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Positions_Companies_CompanyId",
                table: "Positions",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Positions_Companies_CompanyId",
                table: "Positions");

            migrationBuilder.DropIndex(
                name: "IX_Positions_CompanyId",
                table: "Positions");

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "Positions",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }
    }
}
