using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StringAnalyzer.API.Migrations
{
    /// <inheritdoc />
    public partial class ThirdMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StingRecords",
                table: "StingRecords");

            migrationBuilder.RenameTable(
                name: "StingRecords",
                newName: "StringRecords");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StringRecords",
                table: "StringRecords",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StringRecords",
                table: "StringRecords");

            migrationBuilder.RenameTable(
                name: "StringRecords",
                newName: "StingRecords");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StingRecords",
                table: "StingRecords",
                column: "Id");
        }
    }
}
