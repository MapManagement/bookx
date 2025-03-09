using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookxBackend.Migrations
{
    /// <inheritdoc />
    public partial class TypoUserMailAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MailAdresse",
                table: "Users",
                newName: "MailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Users_MailAddress",
                table: "Users",
                column: "MailAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_MailAddress",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "MailAddress",
                table: "Users",
                newName: "MailAdresse");
        }
    }
}
