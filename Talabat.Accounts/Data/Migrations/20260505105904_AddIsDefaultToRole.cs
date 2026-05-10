using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talabat.Accounts.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDefaultToRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                schema: "Accounts",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                schema: "Accounts",
                table: "Roles");
        }
    }
}
