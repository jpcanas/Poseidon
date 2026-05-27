using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Poseidon.Migrations
{
    /// <inheritdoc />
    public partial class FileRecordModuleEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModuleDocumentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModuleId = table.Column<int>(type: "integer", nullable: false),
                    DocumentTypeName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleDocumentTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleDocumentTypes_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FileRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileKey = table.Column<string>(type: "text", nullable: false),
                    ThumbnailKey = table.Column<string>(type: "text", nullable: true),
                    OriginalFileName = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    ModuleId = table.Column<int>(type: "integer", nullable: false),
                    ReferenceId = table.Column<int>(type: "integer", nullable: false),
                    ModuleDocumentTypeId = table.Column<int>(type: "integer", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()"),
                    UploadedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileRecords_ModuleDocumentTypes_ModuleDocumentTypeId",
                        column: x => x.ModuleDocumentTypeId,
                        principalTable: "ModuleDocumentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FileRecords_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ModuleDocumentTypes",
                columns: new[] { "Id", "Description", "DocumentTypeName", "ModuleId" },
                values: new object[] { 1, "Profile pictures uploaded by users for their accounts", "Profile Picture", 1 });

            migrationBuilder.CreateIndex(
                name: "IX_FileRecords_ModuleDocumentTypeId",
                table: "FileRecords",
                column: "ModuleDocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FileRecords_ModuleId_ReferenceId",
                table: "FileRecords",
                columns: new[] { "ModuleId", "ReferenceId" });

            migrationBuilder.CreateIndex(
                name: "IX_ModuleDocumentTypes_ModuleId_DocumentTypeName",
                table: "ModuleDocumentTypes",
                columns: new[] { "ModuleId", "DocumentTypeName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileRecords");

            migrationBuilder.DropTable(
                name: "ModuleDocumentTypes");
        }
    }
}
