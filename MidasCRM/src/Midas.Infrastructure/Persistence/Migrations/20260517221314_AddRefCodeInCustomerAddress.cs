using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Midas.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRefCodeInCustomerAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "nova_poshta_city_ref",
                table: "CustomerAddress",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "nova_poshta_warehouse_ref",
                table: "CustomerAddress",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "nova_poshta_city_ref",
                table: "CustomerAddress");

            migrationBuilder.DropColumn(
                name: "nova_poshta_warehouse_ref",
                table: "CustomerAddress");
        }
    }
}
