using Microsoft.EntityFrameworkCore.Migrations;

namespace MedBook.Migrations
{
    public partial class ChangeIndicatorClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PatientId",
                table: "Indicators",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Indicators_PatientId",
                table: "Indicators",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Indicators_Patients_PatientId",
                table: "Indicators",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Indicators_Patients_PatientId",
                table: "Indicators");

            migrationBuilder.DropIndex(
                name: "IX_Indicators_PatientId",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Indicators");
        }
    }
}
