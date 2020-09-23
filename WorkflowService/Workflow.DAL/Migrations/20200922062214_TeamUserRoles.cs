using Microsoft.EntityFrameworkCore.Migrations;

namespace Workflow.DAL.Migrations
{
    public partial class TeamUserRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanCloseGoals",
                table: "TeamUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanEditGoals",
                table: "TeamUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanEditUsers",
                table: "TeamUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanCloseGoals",
                table: "TeamUsers");

            migrationBuilder.DropColumn(
                name: "CanEditGoals",
                table: "TeamUsers");

            migrationBuilder.DropColumn(
                name: "CanEditUsers",
                table: "TeamUsers");
        }
    }
}
