using Microsoft.EntityFrameworkCore.Migrations;

namespace BookCreator.Data.Migrations
{
    public partial class AddedRatings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsersRatings",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Rating = table.Column<double>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersRatings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BooksRatings",
                columns: table => new
                {
                    RatingId = table.Column<string>(nullable: false),
                    BookId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BooksRatings", x => new { x.BookId, x.RatingId });
                    table.ForeignKey(
                        name: "FK_BooksRatings_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BooksRatings_UsersRatings_RatingId",
                        column: x => x.RatingId,
                        principalTable: "UsersRatings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BooksRatings_RatingId",
                table: "BooksRatings",
                column: "RatingId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersRatings_UserId",
                table: "UsersRatings",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BooksRatings");

            migrationBuilder.DropTable(
                name: "UsersRatings");
        }
    }
}
