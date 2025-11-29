using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talabat.Vender.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCountryIdToVendor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Venders",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "Venders");

            migrationBuilder.RenameColumn(
                name: "TaxCategoryId",
                table: "Products",
                newName: "TaxId");

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "Venders",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Venders");

            migrationBuilder.RenameColumn(
                name: "TaxId",
                table: "Products",
                newName: "TaxCategoryId");

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "Venders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Venders",
                columns: new[] { "Id", "CountryCode", "Email", "Name" },
                values: new object[] { 1, "SA", "vendor1@g.com", "Vendor 1" });
        }
    }
}
