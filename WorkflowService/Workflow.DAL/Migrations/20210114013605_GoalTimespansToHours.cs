using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Workflow.DAL.Migrations
{
    public partial class GoalTimespansToHours : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualPerformingTime",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "EstimatedPerformingTime",
                table: "Goals");

            migrationBuilder.AddColumn<double>(
                name: "ActualPerformingHours",
                table: "Goals",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "EstimatedPerformingHours",
                table: "Goals",
                type: "float",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualPerformingHours",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "EstimatedPerformingHours",
                table: "Goals");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ActualPerformingTime",
                table: "Goals",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EstimatedPerformingTime",
                table: "Goals",
                type: "time",
                nullable: true);
        }
    }
}
