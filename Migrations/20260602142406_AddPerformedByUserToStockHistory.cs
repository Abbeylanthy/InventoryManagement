using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformedByUserToStockHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PerformedByUserId",
                table: "StockHistories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockHistories_PerformedByUserId",
                table: "StockHistories",
                column: "PerformedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockHistories_Users_PerformedByUserId",
                table: "StockHistories",
                column: "PerformedByUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockHistories_Users_PerformedByUserId",
                table: "StockHistories");

            migrationBuilder.DropIndex(
                name: "IX_StockHistories_PerformedByUserId",
                table: "StockHistories");

            migrationBuilder.DropColumn(
                name: "PerformedByUserId",
                table: "StockHistories");
        }
    }
}
