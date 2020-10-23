using Microsoft.EntityFrameworkCore.Migrations;

namespace Woodchuck.Migrations
{
    public partial class addextracolumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShortMessage",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "User",
                table: "Log",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShortMessage",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "User",
                table: "Log");
        }
    }
}
