using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookxBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddFkTagToOwnedBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OwnedBookTag",
                columns: table => new
                {
                    TagsId = table.Column<int>(type: "integer", nullable: false),
                    OwnedBooksUserId = table.Column<int>(type: "integer", nullable: false),
                    OwnedBooksBookIsbn = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnedBookTag", x => new { x.TagsId, x.OwnedBooksUserId, x.OwnedBooksBookIsbn });
                    table.ForeignKey(
                        name: "FK_OwnedBookTag_OwnedBooks_OwnedBooksUserId_OwnedBooksBookIsbn",
                        columns: x => new { x.OwnedBooksUserId, x.OwnedBooksBookIsbn },
                        principalTable: "OwnedBooks",
                        principalColumns: new[] { "UserId", "BookIsbn" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OwnedBookTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OwnedBookTag_OwnedBooksUserId_OwnedBooksBookIsbn",
                table: "OwnedBookTag",
                columns: new[] { "OwnedBooksUserId", "OwnedBooksBookIsbn" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OwnedBookTag");
        }
    }
}
