using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HUTECH_Hospital.Migrations
{
    /// <inheritdoc />
    public partial class AddHealthSurvey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HealthSurveys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<double>(type: "float", nullable: true),
                    Weight = table.Column<double>(type: "float", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BloodType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonalHistory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Allergies = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentMedication = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FamilyHistory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnderlyingConditions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentSymptoms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiscomfortLevel = table.Column<int>(type: "int", nullable: true),
                    SymptomDuration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthSurveys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HealthSurveys_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HealthSurveys_PatientId",
                table: "HealthSurveys",
                column: "PatientId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HealthSurveys");
        }
    }
}
