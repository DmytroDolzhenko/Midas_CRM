using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Midas.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserIntegrationToCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_user_integrations_user_id_provider",
                table: "UserIntegrations");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "UserIntegrations",
                newName: "created_by_user_id");

            migrationBuilder.AddColumn<Guid>(
                name: "company_id",
                table: "UserIntegrations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_user_integrations_company_id",
                table: "UserIntegrations",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_integrations_created_by_user_id",
                table: "UserIntegrations",
                column: "created_by_user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_integrations_company_company_id",
                table: "UserIntegrations",
                column: "company_id",
                principalTable: "Company",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_integrations_user_created_by_user_id",
                table: "UserIntegrations",
                column: "created_by_user_id",
                principalTable: "User",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_integrations_company_company_id",
                table: "UserIntegrations");

            migrationBuilder.DropForeignKey(
                name: "fk_user_integrations_user_created_by_user_id",
                table: "UserIntegrations");

            migrationBuilder.DropIndex(
                name: "ix_user_integrations_company_id",
                table: "UserIntegrations");

            migrationBuilder.DropIndex(
                name: "ix_user_integrations_created_by_user_id",
                table: "UserIntegrations");

            migrationBuilder.DropColumn(
                name: "company_id",
                table: "UserIntegrations");

            migrationBuilder.RenameColumn(
                name: "created_by_user_id",
                table: "UserIntegrations",
                newName: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_integrations_user_id_provider",
                table: "UserIntegrations",
                columns: new[] { "user_id", "provider" },
                unique: true);
        }
    }
}
