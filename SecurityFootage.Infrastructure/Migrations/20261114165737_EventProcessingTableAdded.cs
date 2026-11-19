using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecurityFootage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EventProcessingTableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventQueues_Locations_LocationId",
                table: "EventQueues");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "EventQueues");

            migrationBuilder.AlterColumn<long>(
                name: "LocationId",
                table: "EventQueues",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EventQueues_Locations_LocationId",
                table: "EventQueues",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventQueues_Locations_LocationId",
                table: "EventQueues");

            migrationBuilder.AlterColumn<long>(
                name: "LocationId",
                table: "EventQueues",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "EventQueues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_EventQueues_Locations_LocationId",
                table: "EventQueues",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");
        }
    }
}
