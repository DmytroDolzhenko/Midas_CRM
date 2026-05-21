using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Midas.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFinancialOperation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinancialOperation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    operation_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_financial_operation", x => x.id);
                    table.ForeignKey(
                        name: "fk_financial_operation_asp_net_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_financial_operation_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "Order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_financial_operation_company_id",
                table: "FinancialOperation",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "ix_financial_operation_created_by_user_id",
                table: "FinancialOperation",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_financial_operation_order_id",
                table: "FinancialOperation",
                column: "order_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinancialOperation");
        }
    }
}
