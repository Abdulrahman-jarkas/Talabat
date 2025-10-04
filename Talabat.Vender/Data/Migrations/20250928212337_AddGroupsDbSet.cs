using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talabat.Vender.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupsDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModifierGroupProduct_ModifierGroup_ModifierGroupsId",
                table: "ModifierGroupProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModifierGroup",
                table: "ModifierGroup");

            migrationBuilder.RenameTable(
                name: "ModifierGroup",
                newName: "ModifierGroups");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModifierGroups",
                table: "ModifierGroups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ModifierGroupProduct_ModifierGroups_ModifierGroupsId",
                table: "ModifierGroupProduct",
                column: "ModifierGroupsId",
                principalTable: "ModifierGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModifierGroupProduct_ModifierGroups_ModifierGroupsId",
                table: "ModifierGroupProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModifierGroups",
                table: "ModifierGroups");

            migrationBuilder.RenameTable(
                name: "ModifierGroups",
                newName: "ModifierGroup");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModifierGroup",
                table: "ModifierGroup",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ModifierGroupProduct_ModifierGroup_ModifierGroupsId",
                table: "ModifierGroupProduct",
                column: "ModifierGroupsId",
                principalTable: "ModifierGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
