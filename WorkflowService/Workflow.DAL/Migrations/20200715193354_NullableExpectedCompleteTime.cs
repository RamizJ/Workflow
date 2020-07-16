using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Workflow.DAL.Migrations
{
    public partial class NullableExpectedCompleteTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedCompletedDate",
                table: "Projects",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpectedCompletedDate",
                table: "Goals",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpectedCompletedDate",
                table: "Projects");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpectedCompletedDate",
                table: "Goals",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
