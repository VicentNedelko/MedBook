using Microsoft.EntityFrameworkCore.Migrations;

namespace MedBook.Migrations
{
    public partial class AddBearingIndicator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BearingIndicatorId",
                table: "SampleIndicators",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BearingIndicators",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BearingIndicators", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SampleIndicators_BearingIndicatorId",
                table: "SampleIndicators",
                column: "BearingIndicatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_SampleIndicators_BearingIndicators_BearingIndicatorId",
                table: "SampleIndicators",
                column: "BearingIndicatorId",
                principalTable: "BearingIndicators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SampleIndicators_BearingIndicators_BearingIndicatorId",
                table: "SampleIndicators");

            migrationBuilder.DropTable(
                name: "BearingIndicators");

            migrationBuilder.DropIndex(
                name: "IX_SampleIndicators_BearingIndicatorId",
                table: "SampleIndicators");

            migrationBuilder.DropColumn(
                name: "BearingIndicatorId",
                table: "SampleIndicators");
        }
    }
}
