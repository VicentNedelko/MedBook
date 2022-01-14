using Microsoft.EntityFrameworkCore.Migrations;

namespace MedBook.Migrations
{
    public partial class AddReferencesToBearing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ReferenceMax",
                table: "BearingIndicators",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ReferenceMin",
                table: "BearingIndicators",
                type: "float",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceMax",
                table: "BearingIndicators");

            migrationBuilder.DropColumn(
                name: "ReferenceMin",
                table: "BearingIndicators");
        }
    }
}
