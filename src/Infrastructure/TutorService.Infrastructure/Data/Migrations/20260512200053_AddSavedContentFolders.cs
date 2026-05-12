using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutorService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSavedContentFolders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FolderId",
                table: "SavedContents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SavedContentFolders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TutorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedContentFolders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedContentFolders_TutorProfiles_TutorId",
                        column: x => x.TutorId,
                        principalTable: "TutorProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedContents_FolderId",
                table: "SavedContents",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedContentFolders_TutorId",
                table: "SavedContentFolders",
                column: "TutorId");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedContents_SavedContentFolders_FolderId",
                table: "SavedContents",
                column: "FolderId",
                principalTable: "SavedContentFolders",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedContents_SavedContentFolders_FolderId",
                table: "SavedContents");

            migrationBuilder.DropTable(
                name: "SavedContentFolders");

            migrationBuilder.DropIndex(
                name: "IX_SavedContents_FolderId",
                table: "SavedContents");

            migrationBuilder.DropColumn(
                name: "FolderId",
                table: "SavedContents");
        }
    }
}
