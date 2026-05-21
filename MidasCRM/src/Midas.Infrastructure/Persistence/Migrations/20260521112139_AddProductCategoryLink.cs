using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Midas.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductCategoryLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_product_product_category_product_category_id",
                table: "Product");

            migrationBuilder.AlterColumn<int>(
                name: "product_category_id",
                table: "Product",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "ProductCategoryLink",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "integer", nullable: false),
                    category_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_category_link", x => new { x.product_id, x.category_id });
                    table.ForeignKey(
                        name: "fk_product_category_link_product_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "ProductCategory",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_product_category_link_products_product_id",
                        column: x => x.product_id,
                        principalTable: "Product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_product_category_link_category_id",
                table: "ProductCategoryLink",
                column: "category_id");

            migrationBuilder.AddForeignKey(
                name: "fk_product_product_category_product_category_id",
                table: "Product",
                column: "product_category_id",
                principalTable: "ProductCategory",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_product_product_category_product_category_id",
                table: "Product");

            migrationBuilder.DropTable(
                name: "ProductCategoryLink");

            migrationBuilder.AlterColumn<int>(
                name: "product_category_id",
                table: "Product",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_product_product_category_product_category_id",
                table: "Product",
                column: "product_category_id",
                principalTable: "ProductCategory",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
