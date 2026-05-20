using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Midas.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNovaPoshtaSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_sender_address_user_logistic_profile_user_logistic_pro",
                table: "user_sender_address");

            migrationBuilder.AddForeignKey(
                name: "fk_user_sender_address_user_logistic_profiles_user_logistic_pr",
                table: "user_sender_address",
                column: "user_logistic_profile_id",
                principalTable: "UserLogisticProfiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_sender_address_user_logistic_profiles_user_logistic_pr",
                table: "user_sender_address");

            migrationBuilder.AddForeignKey(
                name: "fk_user_sender_address_user_logistic_profile_user_logistic_pro",
                table: "user_sender_address",
                column: "user_logistic_profile_id",
                principalTable: "UserLogisticProfiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
