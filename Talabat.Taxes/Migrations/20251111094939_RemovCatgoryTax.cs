using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talabat.Taxes.Migrations
{
    /// <inheritdoc />
    public partial class RemovCatgoryTax : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaxPolicies_TaxCategories_ServiceFeeTaxCategoryId",
                table: "TaxPolicies");

            migrationBuilder.DropIndex(
                name: "IX_TaxPolicies_ServiceFeeTaxCategoryId",
                table: "TaxPolicies");

            migrationBuilder.DropColumn(
                name: "ServiceFeeTaxCategoryId",
                table: "TaxPolicies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServiceFeeTaxCategoryId",
                table: "TaxPolicies",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TaxPolicies_ServiceFeeTaxCategoryId",
                table: "TaxPolicies",
                column: "ServiceFeeTaxCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaxPolicies_TaxCategories_ServiceFeeTaxCategoryId",
                table: "TaxPolicies",
                column: "ServiceFeeTaxCategoryId",
                principalTable: "TaxCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
