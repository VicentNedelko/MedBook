using Microsoft.EntityFrameworkCore.Migrations;

namespace MedBook.Migrations
{
    public partial class addFKtoIndicatorsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Indicators_Researches_ResearchId",
                table: "Indicators");

            migrationBuilder.AlterColumn<int>(
                name: "ResearchId",
                table: "Indicators",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Indicators_Researches_ResearchId",
                table: "Indicators",
                column: "ResearchId",
                principalTable: "Researches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Indicators_Researches_ResearchId",
                table: "Indicators");

            migrationBuilder.AlterColumn<int>(
                name: "ResearchId",
                table: "Indicators",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Indicators_Researches_ResearchId",
                table: "Indicators",
                column: "ResearchId",
                principalTable: "Researches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
