using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Data.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class ManyToManyMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthorEntity",
                table: "AuthorEntity");

            migrationBuilder.DropColumn(
                name: "Authors",
                table: "Books");

            migrationBuilder.RenameTable(
                name: "AuthorEntity",
                newName: "Authors");

            migrationBuilder.RenameIndex(
                name: "IX_AuthorEntity_FullName",
                table: "Authors",
                newName: "IX_Authors_FullName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Authors",
                table: "Authors",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AuthorEntityBookEntity",
                columns: table => new
                {
                    AuthorsId = table.Column<Guid>(type: "uuid", nullable: false),
                    BooksId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorEntityBookEntity", x => new { x.AuthorsId, x.BooksId });
                    table.ForeignKey(
                        name: "FK_AuthorEntityBookEntity_Authors_AuthorsId",
                        column: x => x.AuthorsId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthorEntityBookEntity_Books_BooksId",
                        column: x => x.BooksId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorEntityBookEntity_BooksId",
                table: "AuthorEntityBookEntity",
                column: "BooksId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorEntityBookEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Authors",
                table: "Authors");

            migrationBuilder.RenameTable(
                name: "Authors",
                newName: "AuthorEntity");

            migrationBuilder.RenameIndex(
                name: "IX_Authors_FullName",
                table: "AuthorEntity",
                newName: "IX_AuthorEntity_FullName");

            migrationBuilder.AddColumn<string[]>(
                name: "Authors",
                table: "Books",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthorEntity",
                table: "AuthorEntity",
                column: "Id");
        }
    }
}
