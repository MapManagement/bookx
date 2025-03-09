using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookxBackend.Migrations
{
    /// <inheritdoc />
    public partial class NameOfPublisherMutable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Publishers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Publishers");
        }
    }
}
