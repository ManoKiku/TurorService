using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutorService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RelationChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentTutorRelations_Users_UserId1",
                table: "StudentTutorRelations");

            migrationBuilder.DropIndex(
                name: "IX_StudentTutorRelations_UserId1",
                table: "StudentTutorRelations");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "StudentTutorRelations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "StudentTutorRelations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentTutorRelations_UserId1",
                table: "StudentTutorRelations",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentTutorRelations_Users_UserId1",
                table: "StudentTutorRelations",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
