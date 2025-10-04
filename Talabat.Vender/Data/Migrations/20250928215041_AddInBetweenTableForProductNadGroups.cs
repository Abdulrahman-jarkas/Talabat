using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talabat.Vender.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInBetweenTableForProductNadGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModifierGroupProduct_ModifierGroups_ModifierGroupsId",
                table: "ModifierGroupProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_ModifierGroupProduct_Products_ProductId",
                table: "ModifierGroupProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModifierGroupProduct",
                table: "ModifierGroupProduct");

            migrationBuilder.RenameTable(
                name: "ModifierGroupProduct",
                newName: "ProductsModifierGroups");

            migrationBuilder.RenameIndex(
                name: "IX_ModifierGroupProduct_ProductId",
                table: "ProductsModifierGroups",
                newName: "IX_ProductsModifierGroups_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductsModifierGroups",
                table: "ProductsModifierGroups",
                columns: new[] { "ModifierGroupsId", "ProductId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProductsModifierGroups_ModifierGroups_ModifierGroupsId",
                table: "ProductsModifierGroups",
                column: "ModifierGroupsId",
                principalTable: "ModifierGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductsModifierGroups_Products_ProductId",
                table: "ProductsModifierGroups",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductsModifierGroups_ModifierGroups_ModifierGroupsId",
                table: "ProductsModifierGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductsModifierGroups_Products_ProductId",
                table: "ProductsModifierGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductsModifierGroups",
                table: "ProductsModifierGroups");

            migrationBuilder.RenameTable(
                name: "ProductsModifierGroups",
                newName: "ModifierGroupProduct");

            migrationBuilder.RenameIndex(
                name: "IX_ProductsModifierGroups_ProductId",
                table: "ModifierGroupProduct",
                newName: "IX_ModifierGroupProduct_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModifierGroupProduct",
                table: "ModifierGroupProduct",
                columns: new[] { "ModifierGroupsId", "ProductId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ModifierGroupProduct_ModifierGroups_ModifierGroupsId",
                table: "ModifierGroupProduct",
                column: "ModifierGroupsId",
                principalTable: "ModifierGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModifierGroupProduct_Products_ProductId",
                table: "ModifierGroupProduct",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
