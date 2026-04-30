using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdeteCoureseAndEnrollment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseType",
                table: "Course");

            migrationBuilder.AddColumn<int>(
                name: "CourseType",
                table: "CourseEnrollment",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseType",
                table: "CourseEnrollment");

            migrationBuilder.AddColumn<int>(
                name: "CourseType",
                table: "Course",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
