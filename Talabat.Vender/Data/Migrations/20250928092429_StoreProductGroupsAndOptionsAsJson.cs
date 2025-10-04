using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talabat.Vender.Data
{
    /// <inheritdoc />
    public partial class StoreProductGroupsAndOptionsAsJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModifierGroupProduct");

            migrationBuilder.DropColumn(
                name: "ModifierGroupIds",
                table: "Modifiers");

            migrationBuilder.DropColumn(
                name: "ModifierIds",
                table: "ModifierGroups");

            migrationBuilder.AddColumn<string>(
                name: "ModifierGroups",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Modifiers",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.CreateTable(
                name: "ModifierModifierGroup",
                columns: table => new
                {
                    ModifierGroupsId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModifierModifierGroup", x => new { x.ModifierGroupsId, x.ModifiersId });
                    table.ForeignKey(
                        name: "FK_ModifierModifierGroup_ModifierGroups_ModifierGroupsId",
                        column: x => x.ModifierGroupsId,
                        principalTable: "ModifierGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModifierModifierGroup_Modifiers_ModifiersId",
                        column: x => x.ModifiersId,
                        principalTable: "Modifiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModifierModifierGroup_ModifiersId",
                table: "ModifierModifierGroup",
                column: "ModifiersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModifierModifierGroup");

            migrationBuilder.DropColumn(
                name: "ModifierGroups",
                table: "Products");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Modifiers",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<Guid[]>(
                name: "ModifierGroupIds",
                table: "Modifiers",
                type: "uuid[]",
                nullable: false,
                defaultValue: new Guid[0]);

            migrationBuilder.AddColumn<Guid[]>(
                name: "ModifierIds",
                table: "ModifierGroups",
                type: "uuid[]",
                nullable: false,
                defaultValue: new Guid[0]);

            migrationBuilder.CreateTable(
                name: "ModifierGroupProduct",
                columns: table => new
                {
                    ModifierGroupsId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModifierGroupProduct", x => new { x.ModifierGroupsId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_ModifierGroupProduct_ModifierGroups_ModifierGroupsId",
                        column: x => x.ModifierGroupsId,
                        principalTable: "ModifierGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModifierGroupProduct_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModifierGroupProduct_ProductId",
                table: "ModifierGroupProduct",
                column: "ProductId");
        }
    }
}
