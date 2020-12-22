using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Workflow.DAL.Migrations
{
    public partial class GoalMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GoalMessages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    GoalId = table.Column<int>(nullable: false),
                    OwnerId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoalMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoalMessages_Goals_GoalId",
                        column: x => x.GoalId,
                        principalTable: "Goals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoalMessages_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserGoalMessages",
                columns: table => new
                {
                    GoalId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    GoalMessageId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGoalMessages", x => new { x.GoalId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserGoalMessages_Goals_GoalId",
                        column: x => x.GoalId,
                        principalTable: "Goals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGoalMessages_GoalMessages_GoalMessageId",
                        column: x => x.GoalMessageId,
                        principalTable: "GoalMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserGoalMessages_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GoalMessages_GoalId",
                table: "GoalMessages",
                column: "GoalId");

            migrationBuilder.CreateIndex(
                name: "IX_GoalMessages_OwnerId",
                table: "GoalMessages",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGoalMessages_GoalMessageId",
                table: "UserGoalMessages",
                column: "GoalMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGoalMessages_UserId",
                table: "UserGoalMessages",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserGoalMessages");

            migrationBuilder.DropTable(
                name: "GoalMessages");
        }
    }
}
