using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Workflow.DAL.Migrations
{
    public partial class UserGoalMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGoalMessages_Goals_GoalId",
                table: "UserGoalMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGoalMessages_GoalMessages_GoalMessageId",
                table: "UserGoalMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserGoalMessages",
                table: "UserGoalMessages");

            migrationBuilder.DropIndex(
                name: "IX_UserGoalMessages_GoalMessageId",
                table: "UserGoalMessages");

            migrationBuilder.DropColumn(
                name: "GoalId",
                table: "UserGoalMessages");

            migrationBuilder.AlterColumn<int>(
                name: "GoalMessageId",
                table: "UserGoalMessages",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastReadingDate",
                table: "UserGoalMessages",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Goals",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "GoalMessages",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRemoved",
                table: "GoalMessages",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEditDate",
                table: "GoalMessages",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserGoalMessages",
                table: "UserGoalMessages",
                columns: new[] { "GoalMessageId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserGoalMessages_GoalMessages_GoalMessageId",
                table: "UserGoalMessages",
                column: "GoalMessageId",
                principalTable: "GoalMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGoalMessages_GoalMessages_GoalMessageId",
                table: "UserGoalMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserGoalMessages",
                table: "UserGoalMessages");

            migrationBuilder.DropColumn(
                name: "LastReadingDate",
                table: "UserGoalMessages");

            migrationBuilder.DropColumn(
                name: "IsRemoved",
                table: "GoalMessages");

            migrationBuilder.DropColumn(
                name: "LastEditDate",
                table: "GoalMessages");

            migrationBuilder.AlterColumn<int>(
                name: "GoalMessageId",
                table: "UserGoalMessages",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "GoalId",
                table: "UserGoalMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Goals",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "GoalMessages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 2048,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserGoalMessages",
                table: "UserGoalMessages",
                columns: new[] { "GoalId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserGoalMessages_GoalMessageId",
                table: "UserGoalMessages",
                column: "GoalMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGoalMessages_Goals_GoalId",
                table: "UserGoalMessages",
                column: "GoalId",
                principalTable: "Goals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGoalMessages_GoalMessages_GoalMessageId",
                table: "UserGoalMessages",
                column: "GoalMessageId",
                principalTable: "GoalMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
