using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Poseidon.Migrations
{
    /// <inheritdoc />
    public partial class AddedSubmoduleData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SubModules",
                columns: new[] { "Id", "Code", "Description", "ModuleId", "Name" },
                values: new object[,]
                {
                    { 4, "UAC_VIEW_USERLIST", "", 1, "View User list" },
                    { 5, "UAC_VIEW_ROLES", "", 1, "View Roles" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SubModules",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SubModules",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
