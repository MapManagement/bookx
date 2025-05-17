using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookxBackend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIdOwndBooks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "OwnedBooks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "OwnedBooks",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
