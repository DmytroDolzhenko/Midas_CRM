using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Midas.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicProductCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_public",
                table: "ProductCategory",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "UserProductCategory",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_category_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_product_category", x => new { x.user_id, x.product_category_id });
                    table.ForeignKey(
                        name: "fk_user_product_category_product_category_product_category_id",
                        column: x => x.product_category_id,
                        principalTable: "ProductCategory",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_product_category_name",
                table: "ProductCategory",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_product_category_product_category_id",
                table: "UserProductCategory",
                column: "product_category_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProductCategory");

            migrationBuilder.DropIndex(
                name: "ix_product_category_name",
                table: "ProductCategory");

            migrationBuilder.DropColumn(
                name: "is_public",
                table: "ProductCategory");
        }
    }
} 
