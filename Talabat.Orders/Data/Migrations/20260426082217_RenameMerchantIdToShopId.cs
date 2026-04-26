using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talabat.Orders.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameMerchantIdToShopId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MerchantId",
                schema: "Orders",
                table: "Orders",
                newName: "ShopId");

            migrationBuilder.RenameColumn(
                name: "MerchantId",
                schema: "Orders",
                table: "CheckoutSessions",
                newName: "ShopId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "Orders",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShopId_CreatedAt",
                schema: "Orders",
                table: "Orders",
                columns: new[] { "ShopId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_ShopId_CreatedAt",
                schema: "Orders",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "Orders",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "ShopId",
                schema: "Orders",
                table: "Orders",
                newName: "MerchantId");

            migrationBuilder.RenameColumn(
                name: "ShopId",
                schema: "Orders",
                table: "CheckoutSessions",
                newName: "MerchantId");
        }
    }
}
