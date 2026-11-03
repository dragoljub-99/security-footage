using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecurityFootage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFootageName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "FootageInformation",
                newName: "BlobName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BlobName",
                table: "FootageInformation",
                newName: "Name");
        }
    }
}
