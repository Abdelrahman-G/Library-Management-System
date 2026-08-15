using LibraryManagement.Api.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library_Management_System.Data.Migrations;

[DbContext(typeof(LibraryDbContext))]
[Migration("20260815155215_AddTokenVersion")]
public partial class AddTokenVersion : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "TokenVersion",
            table: "SystemUsers",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "TokenVersion",
            table: "SystemUsers");
    }
}
