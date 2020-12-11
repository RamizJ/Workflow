using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Workflow.DAL.Migrations
{
    public partial class ActualPerformingTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "ActualPerformingTime",
                table: "Goals",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualPerformingTime",
                table: "Goals");
        }
    }
}
