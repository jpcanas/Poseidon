using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Poseidon.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusColor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserStatuses",
                keyColumn: "UserStatusId",
                keyValue: 1,
                column: "Color",
                value: "green");

            migrationBuilder.UpdateData(
                table: "UserStatuses",
                keyColumn: "UserStatusId",
                keyValue: 2,
                column: "Color",
                value: "red");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserStatuses",
                keyColumn: "UserStatusId",
                keyValue: 1,
                column: "Color",
                value: "");

            migrationBuilder.UpdateData(
                table: "UserStatuses",
                keyColumn: "UserStatusId",
                keyValue: 2,
                column: "Color",
                value: "");
        }
    }
}
