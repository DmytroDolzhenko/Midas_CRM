using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Midas.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelAfterNovaPoshtaDiFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "weight",
                table: "Product",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "tracking_number",
                table: "Order",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "weight",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "tracking_number",
                table: "Order");
        }
    }
}
