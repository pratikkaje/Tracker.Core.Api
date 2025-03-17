using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tracker.Core.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAmountColumnPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                           name: "Amount",
                           table: "Transactions",
                           type: "decimal(18, 4)",
                           nullable: false,
                           oldClrType: typeof(decimal),
                           oldType: "decimal(10, 4)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Transactions",
                type: "decimal(10, 4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 4)");
        }
    }
}
