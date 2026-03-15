using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryWeb.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUserToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "User",
                newName: "ApplicationUser");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "ApplicationUser",
                newName: "User");
        }
    }
}
