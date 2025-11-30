using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talabat.OrderProcessing.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameCheckoutSessionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_checkoutSessions",
                table: "checkoutSessions");

            migrationBuilder.RenameTable(
                name: "checkoutSessions",
                newName: "CheckoutSessions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CheckoutSessions",
                table: "CheckoutSessions",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CheckoutSessions",
                table: "CheckoutSessions");

            migrationBuilder.RenameTable(
                name: "CheckoutSessions",
                newName: "checkoutSessions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_checkoutSessions",
                table: "checkoutSessions",
                column: "Id");
        }
    }
}
