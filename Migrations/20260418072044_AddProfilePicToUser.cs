using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Poseidon.Migrations
{
    /// <inheritdoc />
    public partial class AddProfilePicToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProfilePictureFileRecordId",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "ProfilePictureFileRecordId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ProfilePictureFileRecordId",
                table: "Users",
                column: "ProfilePictureFileRecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_FileRecords_ProfilePictureFileRecordId",
                table: "Users",
                column: "ProfilePictureFileRecordId",
                principalTable: "FileRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_FileRecords_ProfilePictureFileRecordId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ProfilePictureFileRecordId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProfilePictureFileRecordId",
                table: "Users");
        }
    }
}
