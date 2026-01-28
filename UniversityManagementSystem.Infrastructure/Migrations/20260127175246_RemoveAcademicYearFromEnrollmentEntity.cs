using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAcademicYearFromEnrollmentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Enrollments_StudentId_CourseId_SemesterId_AcademicYear",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "AcademicYear",
                table: "Enrollments");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId_CourseId_SemesterId",
                table: "Enrollments",
                columns: new[] { "StudentId", "CourseId", "SemesterId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Enrollments_StudentId_CourseId_SemesterId",
                table: "Enrollments");

            migrationBuilder.AddColumn<int>(
                name: "AcademicYear",
                table: "Enrollments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId_CourseId_SemesterId_AcademicYear",
                table: "Enrollments",
                columns: new[] { "StudentId", "CourseId", "SemesterId", "AcademicYear" },
                unique: true);
        }
    }
}
