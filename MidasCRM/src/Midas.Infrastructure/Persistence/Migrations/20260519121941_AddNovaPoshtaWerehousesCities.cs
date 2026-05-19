using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Midas.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNovaPoshtaWerehousesCities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_user_integration",
                table: "UserIntegration");

            migrationBuilder.DropColumn(
                name: "value",
                table: "Contact");

            migrationBuilder.RenameTable(
                name: "UserIntegration",
                newName: "UserIntegrations");

            migrationBuilder.RenameIndex(
                name: "ix_user_integration_user_id_provider",
                table: "UserIntegrations",
                newName: "ix_user_integrations_user_id_provider");

            migrationBuilder.AddColumn<string>(
                name: "contact_person_recipient",
                table: "Product",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "phone_number",
                table: "Contact",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_integrations",
                table: "UserIntegrations",
                column: "id");

            migrationBuilder.CreateTable(
                name: "nova_poshta_cities",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    @ref = table.Column<string>(name: "ref", type: "text", nullable: false),
                    description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    settlement_type_description = table.Column<string>(type: "text", nullable: false),
                    area_description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_nova_poshta_cities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "nova_poshta_warehouses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    @ref = table.Column<string>(name: "ref", type: "text", nullable: false),
                    city_ref = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    number = table.Column<int>(type: "integer", nullable: false),
                    warehouse_index = table.Column<string>(type: "text", nullable: false),
                    type_of_warehouse = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_nova_poshta_warehouses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "UserLogisticProfiles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_integration_id = table.Column<int>(type: "integer", nullable: false),
                    sender_ref = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    contact_sender_ref = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    senders_phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_logistic_profiles", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_logistic_profiles_user_integrations_user_integration_id",
                        column: x => x.user_integration_id,
                        principalTable: "UserIntegrations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_sender_address",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_logistic_profile_id = table.Column<int>(type: "integer", nullable: false),
                    city_ref = table.Column<string>(type: "text", nullable: false),
                    address_ref = table.Column<string>(type: "text", nullable: false),
                    warehouse_index = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_sender_address", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_sender_address_user_logistic_profile_user_logistic_pro",
                        column: x => x.user_logistic_profile_id,
                        principalTable: "UserLogisticProfiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_nova_poshta_cities_ref",
                table: "nova_poshta_cities",
                column: "ref",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_nova_poshta_warehouses_city_ref",
                table: "nova_poshta_warehouses",
                column: "city_ref");

            migrationBuilder.CreateIndex(
                name: "ix_nova_poshta_warehouses_ref",
                table: "nova_poshta_warehouses",
                column: "ref",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_sender_address_user_logistic_profile_id",
                table: "user_sender_address",
                column: "user_logistic_profile_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_logistic_profiles_user_integration_id",
                table: "UserLogisticProfiles",
                column: "user_integration_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "nova_poshta_cities");

            migrationBuilder.DropTable(
                name: "nova_poshta_warehouses");

            migrationBuilder.DropTable(
                name: "user_sender_address");

            migrationBuilder.DropTable(
                name: "UserLogisticProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user_integrations",
                table: "UserIntegrations");

            migrationBuilder.DropColumn(
                name: "contact_person_recipient",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "phone_number",
                table: "Contact");

            migrationBuilder.RenameTable(
                name: "UserIntegrations",
                newName: "UserIntegration");

            migrationBuilder.RenameIndex(
                name: "ix_user_integrations_user_id_provider",
                table: "UserIntegration",
                newName: "ix_user_integration_user_id_provider");

            migrationBuilder.AddColumn<string>(
                name: "value",
                table: "Contact",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_integration",
                table: "UserIntegration",
                column: "id");
        }
    }
}
