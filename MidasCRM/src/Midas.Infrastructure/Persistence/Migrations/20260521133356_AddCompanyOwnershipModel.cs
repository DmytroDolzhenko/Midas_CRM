using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Midas.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyOwnershipModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "owner_id",
                table: "Warehouse",
                newName: "company_id");

            migrationBuilder.RenameColumn(
                name: "owner_id",
                table: "ProductVariant",
                newName: "company_id");

            migrationBuilder.RenameColumn(
                name: "owner_id",
                table: "ProductImages",
                newName: "company_id");

            migrationBuilder.RenameIndex(
                name: "ix_product_images_owner_id",
                table: "ProductImages",
                newName: "ix_product_images_company_id");

            migrationBuilder.RenameColumn(
                name: "owner_id",
                table: "Product",
                newName: "company_id");

            migrationBuilder.RenameColumn(
                name: "owner_id",
                table: "Payment",
                newName: "company_id");

            migrationBuilder.RenameColumn(
                name: "owner_id",
                table: "OrderSource",
                newName: "company_id");

            migrationBuilder.RenameColumn(
                name: "owner_id",
                table: "OrderItem",
                newName: "company_id");

            migrationBuilder.RenameColumn(
                name: "owner_id",
                table: "Order",
                newName: "company_id");

            migrationBuilder.RenameColumn(
                name: "owner_id",
                table: "CustomerAddress",
                newName: "company_id");

            migrationBuilder.RenameColumn(
                name: "owner_id",
                table: "Customer",
                newName: "company_id");

            migrationBuilder.RenameColumn(
                name: "owner_id",
                table: "Contact",
                newName: "company_id");

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    tax_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_company", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyMember",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    joined_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_company_member", x => x.id);
                    table.ForeignKey(
                        name: "fk_company_member_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_company_member_company_company_id",
                        column: x => x.company_id,
                        principalTable: "Company",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_company_member_company_id_user_id",
                table: "CompanyMember",
                columns: new[] { "company_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_company_member_user_id",
                table: "CompanyMember",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyMember");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "Warehouse",
                newName: "owner_id");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "ProductVariant",
                newName: "owner_id");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "ProductImages",
                newName: "owner_id");

            migrationBuilder.RenameIndex(
                name: "ix_product_images_company_id",
                table: "ProductImages",
                newName: "ix_product_images_owner_id");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "Product",
                newName: "owner_id");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "Payment",
                newName: "owner_id");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "OrderSource",
                newName: "owner_id");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "OrderItem",
                newName: "owner_id");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "Order",
                newName: "owner_id");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "CustomerAddress",
                newName: "owner_id");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "Customer",
                newName: "owner_id");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "Contact",
                newName: "owner_id");
        }
    }
}
