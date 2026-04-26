using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talabat.Products.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameMerchantIdToShopId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MerchantId",
                schema: "Products",
                table: "Products",
                newName: "ShopId");

            migrationBuilder.CreateTable(
                name: "Shops",
                schema: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shops", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Shops",
                schema: "Products");

            migrationBuilder.RenameColumn(
                name: "ShopId",
                schema: "Products",
                table: "Products",
                newName: "MerchantId");
        }
    }
}
