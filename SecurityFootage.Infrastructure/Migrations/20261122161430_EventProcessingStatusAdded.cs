using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecurityFootage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EventProcessingStatusAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "EventQueues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "EventQueues");
        }
    }
}
