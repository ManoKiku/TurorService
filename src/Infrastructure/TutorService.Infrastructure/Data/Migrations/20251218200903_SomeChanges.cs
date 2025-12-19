using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutorService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SomeChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileUrl",
                table: "Assignments",
                newName: "MongoFileId");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Assignments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "Assignments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "Assignments");

            migrationBuilder.RenameColumn(
                name: "MongoFileId",
                table: "Assignments",
                newName: "FileUrl");
        }
    }
}
