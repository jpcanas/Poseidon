using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Poseidon.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRolePermissionDefinition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_Modules_ModuleId",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_ModuleId",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "ModuleId",
                table: "RolePermissions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ModuleId",
                table: "RolePermissions",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "ModuleId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "ModuleId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "ModuleId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_ModuleId",
                table: "RolePermissions",
                column: "ModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_Modules_ModuleId",
                table: "RolePermissions",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id");
        }
    }
}
