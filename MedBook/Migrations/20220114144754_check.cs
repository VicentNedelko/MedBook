using Microsoft.EntityFrameworkCore.Migrations;

namespace MedBook.Migrations
{
    public partial class check : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cure_Prescription_PrescriptionId",
                table: "Cure");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescription_Visit_VisitId",
                table: "Prescription");

            migrationBuilder.DropForeignKey(
                name: "FK_Researches_Visit_VisitId",
                table: "Researches");

            migrationBuilder.DropForeignKey(
                name: "FK_Visit_Doctors_DoctorId",
                table: "Visit");

            migrationBuilder.DropForeignKey(
                name: "FK_Visit_Patients_PatientId",
                table: "Visit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Visit",
                table: "Visit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Prescription",
                table: "Prescription");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cure",
                table: "Cure");

            migrationBuilder.RenameTable(
                name: "Visit",
                newName: "Visits");

            migrationBuilder.RenameTable(
                name: "Prescription",
                newName: "Prescriptions");

            migrationBuilder.RenameTable(
                name: "Cure",
                newName: "Cures");

            migrationBuilder.RenameIndex(
                name: "IX_Visit_PatientId",
                table: "Visits",
                newName: "IX_Visits_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Visit_DoctorId",
                table: "Visits",
                newName: "IX_Visits_DoctorId");

            migrationBuilder.RenameIndex(
                name: "IX_Prescription_VisitId",
                table: "Prescriptions",
                newName: "IX_Prescriptions_VisitId");

            migrationBuilder.RenameIndex(
                name: "IX_Cure_PrescriptionId",
                table: "Cures",
                newName: "IX_Cures_PrescriptionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Visits",
                table: "Visits",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Prescriptions",
                table: "Prescriptions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cures",
                table: "Cures",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cures_Prescriptions_PrescriptionId",
                table: "Cures",
                column: "PrescriptionId",
                principalTable: "Prescriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prescriptions_Visits_VisitId",
                table: "Prescriptions",
                column: "VisitId",
                principalTable: "Visits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Researches_Visits_VisitId",
                table: "Researches",
                column: "VisitId",
                principalTable: "Visits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Doctors_DoctorId",
                table: "Visits",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Patients_PatientId",
                table: "Visits",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cures_Prescriptions_PrescriptionId",
                table: "Cures");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescriptions_Visits_VisitId",
                table: "Prescriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Researches_Visits_VisitId",
                table: "Researches");

            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Doctors_DoctorId",
                table: "Visits");

            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Patients_PatientId",
                table: "Visits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Visits",
                table: "Visits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Prescriptions",
                table: "Prescriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cures",
                table: "Cures");

            migrationBuilder.RenameTable(
                name: "Visits",
                newName: "Visit");

            migrationBuilder.RenameTable(
                name: "Prescriptions",
                newName: "Prescription");

            migrationBuilder.RenameTable(
                name: "Cures",
                newName: "Cure");

            migrationBuilder.RenameIndex(
                name: "IX_Visits_PatientId",
                table: "Visit",
                newName: "IX_Visit_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Visits_DoctorId",
                table: "Visit",
                newName: "IX_Visit_DoctorId");

            migrationBuilder.RenameIndex(
                name: "IX_Prescriptions_VisitId",
                table: "Prescription",
                newName: "IX_Prescription_VisitId");

            migrationBuilder.RenameIndex(
                name: "IX_Cures_PrescriptionId",
                table: "Cure",
                newName: "IX_Cure_PrescriptionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Visit",
                table: "Visit",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Prescription",
                table: "Prescription",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cure",
                table: "Cure",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cure_Prescription_PrescriptionId",
                table: "Cure",
                column: "PrescriptionId",
                principalTable: "Prescription",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prescription_Visit_VisitId",
                table: "Prescription",
                column: "VisitId",
                principalTable: "Visit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Researches_Visit_VisitId",
                table: "Researches",
                column: "VisitId",
                principalTable: "Visit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Visit_Doctors_DoctorId",
                table: "Visit",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Visit_Patients_PatientId",
                table: "Visit",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
