using System;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.UserCredentials.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef.Migrations
{
    [DbContext(typeof(UserServiceDbContext))]
    [Migration("20210317144051_AddPropertyCreatedAt")]
    public class AddPropertyCreatedAt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: nameof(DbUser.CreatedAt),
                table: DbUser.TableName,
                nullable: false,
                defaultValue: DateTime.Now
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: nameof(DbUser.CreatedAt),
                table: DbUser.TableName
            );
        }

        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
        }
    }
}