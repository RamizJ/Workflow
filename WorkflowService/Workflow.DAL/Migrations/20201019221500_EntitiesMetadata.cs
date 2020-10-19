using Microsoft.EntityFrameworkCore.Migrations;

namespace Workflow.DAL.Migrations
{
    public partial class EntitiesMetadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "Metadata",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Metadata",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Metadata_GroupId",
                table: "Metadata",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Metadata_ProjectId",
                table: "Metadata",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Metadata_Groups_GroupId",
                table: "Metadata",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Metadata_Projects_ProjectId",
                table: "Metadata",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Metadata_Groups_GroupId",
                table: "Metadata");

            migrationBuilder.DropForeignKey(
                name: "FK_Metadata_Projects_ProjectId",
                table: "Metadata");

            migrationBuilder.DropIndex(
                name: "IX_Metadata_GroupId",
                table: "Metadata");

            migrationBuilder.DropIndex(
                name: "IX_Metadata_ProjectId",
                table: "Metadata");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Metadata");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Metadata");
        }
    }
}
