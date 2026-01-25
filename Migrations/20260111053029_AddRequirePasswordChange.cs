using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Poseidon.Migrations
{
    /// <inheritdoc />
    public partial class AddRequirePasswordChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequiredPasswordChange",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiredPasswordChange",
                table: "Users");
        }
    }
}
