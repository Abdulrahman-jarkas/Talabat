using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talabat.Accounts.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsActiveFromAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Accounts_UserId",
                schema: "Accounts",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Accounts",
                table: "Accounts");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UserId",
                schema: "Accounts",
                table: "Accounts",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Accounts_UserId",
                schema: "Accounts",
                table: "Accounts");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Accounts",
                table: "Accounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UserId",
                schema: "Accounts",
                table: "Accounts",
                column: "UserId",
                unique: true,
                filter: "[IsActive] = 1 AND [IsDeleted] = 0");
        }
    }
}
