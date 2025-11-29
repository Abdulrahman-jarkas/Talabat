using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talabat.OrderProcessing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceFees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ServiceFees",
                table: "Orders",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceFees",
                table: "Orders");
        }
    }
}
