using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Talabat.Taxes.Migrations
{
    /// <inheritdoc />
    public partial class AddTaxTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaxCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    VatPercentage = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxPolicyId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxPolicies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CountryCode = table.Column<string>(type: "text", nullable: false),
                    ServiceFeeTaxCategoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxPolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaxPolicies_TaxCategories_ServiceFeeTaxCategoryId",
                        column: x => x.ServiceFeeTaxCategoryId,
                        principalTable: "TaxCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaxCategories_TaxPolicyId",
                table: "TaxCategories",
                column: "TaxPolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxPolicies_ServiceFeeTaxCategoryId",
                table: "TaxPolicies",
                column: "ServiceFeeTaxCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaxCategories_TaxPolicies_TaxPolicyId",
                table: "TaxCategories",
                column: "TaxPolicyId",
                principalTable: "TaxPolicies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaxCategories_TaxPolicies_TaxPolicyId",
                table: "TaxCategories");

            migrationBuilder.DropTable(
                name: "TaxPolicies");

            migrationBuilder.DropTable(
                name: "TaxCategories");
        }
    }
}
