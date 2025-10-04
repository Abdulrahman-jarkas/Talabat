using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talabat.Vender.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRelationShipBetweenGroupAndModifiers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifierGroups",
                table: "Products");

            migrationBuilder.CreateTable(
                name: "ModifierGroup",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Min = table.Column<int>(type: "integer", nullable: false),
                    Max = table.Column<int>(type: "integer", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModifierGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modifiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modifiers", x => x.Id);
                });

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
                        name: "FK_ModifierGroupProduct_ModifierGroup_ModifierGroupsId",
                        column: x => x.ModifierGroupsId,
                        principalTable: "ModifierGroup",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModifierGroupProduct");

            migrationBuilder.DropTable(
                name: "Modifiers");

            migrationBuilder.DropTable(
                name: "ModifierGroup");

            migrationBuilder.AddColumn<string>(
                name: "ModifierGroups",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
