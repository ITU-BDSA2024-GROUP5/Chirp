using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Razor.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cheeps_Authors_AuthorID",
                table: "Cheeps");

            migrationBuilder.RenameColumn(
                name: "AuthorID",
                table: "Cheeps",
                newName: "AuthorId");

            migrationBuilder.RenameColumn(
                name: "CheepID",
                table: "Cheeps",
                newName: "CheepId");

            migrationBuilder.RenameIndex(
                name: "IX_Cheeps_AuthorID",
                table: "Cheeps",
                newName: "IX_Cheeps_AuthorId");

            migrationBuilder.RenameColumn(
                name: "AuthorID",
                table: "Authors",
                newName: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cheeps_Authors_AuthorId",
                table: "Cheeps",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "AuthorId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cheeps_Authors_AuthorId",
                table: "Cheeps");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Cheeps",
                newName: "AuthorID");

            migrationBuilder.RenameColumn(
                name: "CheepId",
                table: "Cheeps",
                newName: "CheepID");

            migrationBuilder.RenameIndex(
                name: "IX_Cheeps_AuthorId",
                table: "Cheeps",
                newName: "IX_Cheeps_AuthorID");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Authors",
                newName: "AuthorID");

            migrationBuilder.AddForeignKey(
                name: "FK_Cheeps_Authors_AuthorID",
                table: "Cheeps",
                column: "AuthorID",
                principalTable: "Authors",
                principalColumn: "AuthorID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
