using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talabat.Vender.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveModifierANdModifierGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModifierModifierGroup");

            migrationBuilder.DropTable(
                name: "ModifierGroups");

            migrationBuilder.DropTable(
                name: "Modifiers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModifierGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Max = table.Column<int>(type: "integer", nullable: false),
                    Min = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModifierGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modifiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modifiers", x => x.Id);
                });

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
    }
}
