using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talabat.Accounts.Data.Migrations
{
    /// <inheritdoc />
    public partial class EnhanceAccountsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS [IX_Roles_Name_TenantId] ON [Accounts].[Roles]");

            migrationBuilder.DropColumn(
                name: "AccountRoles",
                schema: "Accounts",
                table: "Accounts");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "Accounts",
                table: "Roles",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                schema: "Accounts",
                table: "Roles",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.CreateTable(
                name: "AccountAssignments",
                schema: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountAssignments_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "Accounts",
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountAssignments_AccountId",
                schema: "Accounts",
                table: "AccountAssignments",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountAssignments_RoleId",
                schema: "Accounts",
                table: "AccountAssignments",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name_TenantId",
                schema: "Accounts",
                table: "Roles",
                columns: new[] { "Name", "TenantId" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Roles_Name_TenantId",
                schema: "Accounts",
                table: "Roles");

            migrationBuilder.DropTable(
                name: "AccountAssignments",
                schema: "Accounts");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "Accounts",
                table: "Roles");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "Accounts",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<string>(
                name: "AccountRoles",
                schema: "Accounts",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
