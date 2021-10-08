using Microsoft.EntityFrameworkCore.Migrations;

namespace MedBook.Migrations
{
    public partial class AddReferences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ReferenceMax",
                table: "SampleIndicators",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ReferenceMin",
                table: "SampleIndicators",
                type: "float",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceMax",
                table: "SampleIndicators");

            migrationBuilder.DropColumn(
                name: "ReferenceMin",
                table: "SampleIndicators");
        }
    }
}
