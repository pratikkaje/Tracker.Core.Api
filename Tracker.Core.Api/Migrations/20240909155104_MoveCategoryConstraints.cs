using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tracker.Core.Api.Migrations
{
    /// <inheritdoc />
    public partial class MoveCategoryConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "Users",
                newName: "UpdatedDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Users",
                newName: "ModifiedDate");
        }
    }
}
