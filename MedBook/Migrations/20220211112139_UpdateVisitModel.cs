using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MedBook.Migrations
{
    public partial class UpdateVisitModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Visits",
                newName: "Start");

            migrationBuilder.AddColumn<DateTime>(
                name: "End",
                table: "Visits",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "End",
                table: "Visits");

            migrationBuilder.RenameColumn(
                name: "Start",
                table: "Visits",
                newName: "Date");
        }
    }
}
