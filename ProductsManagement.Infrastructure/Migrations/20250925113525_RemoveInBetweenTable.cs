using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductsManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveInBetweenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductModifierGroups_ModifierGroup_ModifierGroupsId",
                table: "ProductModifierGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductModifierGroups_Products_ProductId",
                table: "ProductModifierGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductModifierGroups",
                table: "ProductModifierGroups");

            migrationBuilder.RenameTable(
                name: "ProductModifierGroups",
                newName: "ModifierGroupProduct");

            migrationBuilder.RenameIndex(
                name: "IX_ProductModifierGroups_ProductId",
                table: "ModifierGroupProduct",
                newName: "IX_ModifierGroupProduct_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModifierGroupProduct",
                table: "ModifierGroupProduct",
                columns: new[] { "ModifierGroupsId", "ProductId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ModifierGroupProduct_ModifierGroup_ModifierGroupsId",
                table: "ModifierGroupProduct",
                column: "ModifierGroupsId",
                principalTable: "ModifierGroup",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModifierGroupProduct_ModifierGroup_ModifierGroupsId",
                table: "ModifierGroupProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_ModifierGroupProduct_Products_ProductId",
                table: "ModifierGroupProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModifierGroupProduct",
                table: "ModifierGroupProduct");

            migrationBuilder.RenameTable(
                name: "ModifierGroupProduct",
                newName: "ProductModifierGroups");

            migrationBuilder.RenameIndex(
                name: "IX_ModifierGroupProduct_ProductId",
                table: "ProductModifierGroups",
                newName: "IX_ProductModifierGroups_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductModifierGroups",
                table: "ProductModifierGroups",
                columns: new[] { "ModifierGroupsId", "ProductId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProductModifierGroups_ModifierGroup_ModifierGroupsId",
                table: "ProductModifierGroups",
                column: "ModifierGroupsId",
                principalTable: "ModifierGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductModifierGroups_Products_ProductId",
                table: "ProductModifierGroups",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
