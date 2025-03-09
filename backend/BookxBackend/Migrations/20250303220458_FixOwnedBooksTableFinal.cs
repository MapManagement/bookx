using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BookxBackend.Migrations
{
    /// <inheritdoc />
    public partial class FixOwnedBooksTableFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OwnedBooks_Books_BookIsbn",
                table: "OwnedBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_OwnedBooks_Books_BookIsbn1",
                table: "OwnedBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_OwnedBooks_Users_UserId1",
                table: "OwnedBooks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OwnedBooks",
                table: "OwnedBooks");

            migrationBuilder.DropIndex(
                name: "IX_OwnedBooks_BookIsbn1",
                table: "OwnedBooks");

            migrationBuilder.DropIndex(
                name: "IX_OwnedBooks_UserId",
                table: "OwnedBooks");

            migrationBuilder.DropIndex(
                name: "IX_OwnedBooks_UserId1",
                table: "OwnedBooks");

            migrationBuilder.DropColumn(
                name: "BookIsbn1",
                table: "OwnedBooks");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "OwnedBooks");

            migrationBuilder.AlterColumn<string>(
                name: "BookIsbn",
                table: "OwnedBooks",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "OwnedBooks",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OwnedBooks",
                table: "OwnedBooks",
                columns: new[] { "UserId", "BookIsbn" });

            migrationBuilder.AddForeignKey(
                name: "FK_OwnedBooks_Books_BookIsbn",
                table: "OwnedBooks",
                column: "BookIsbn",
                principalTable: "Books",
                principalColumn: "Isbn",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OwnedBooks_Books_BookIsbn",
                table: "OwnedBooks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OwnedBooks",
                table: "OwnedBooks");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "OwnedBooks",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "BookIsbn",
                table: "OwnedBooks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "BookIsbn1",
                table: "OwnedBooks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "OwnedBooks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OwnedBooks",
                table: "OwnedBooks",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OwnedBooks_BookIsbn1",
                table: "OwnedBooks",
                column: "BookIsbn1");

            migrationBuilder.CreateIndex(
                name: "IX_OwnedBooks_UserId",
                table: "OwnedBooks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnedBooks_UserId1",
                table: "OwnedBooks",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_OwnedBooks_Books_BookIsbn",
                table: "OwnedBooks",
                column: "BookIsbn",
                principalTable: "Books",
                principalColumn: "Isbn");

            migrationBuilder.AddForeignKey(
                name: "FK_OwnedBooks_Books_BookIsbn1",
                table: "OwnedBooks",
                column: "BookIsbn1",
                principalTable: "Books",
                principalColumn: "Isbn");

            migrationBuilder.AddForeignKey(
                name: "FK_OwnedBooks_Users_UserId1",
                table: "OwnedBooks",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
