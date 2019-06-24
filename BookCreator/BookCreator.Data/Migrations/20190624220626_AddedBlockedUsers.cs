using Microsoft.EntityFrameworkCore.Migrations;

namespace BookCreator.Data.Migrations
{
    public partial class AddedBlockedUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlockedUsers_AspNetUsers_BookCreatorUserId1",
                table: "BlockedUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_AspNetUsers_AuthorId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_BlockedUsers_BookCreatorUserId1",
                table: "BlockedUsers");

            migrationBuilder.DropColumn(
                name: "BookCreatorUserId1",
                table: "BlockedUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Books",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Summary",
                table: "Books",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlockedUsers_BlockedUserId",
                table: "BlockedUsers",
                column: "BlockedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlockedUsers_AspNetUsers_BlockedUserId",
                table: "BlockedUsers",
                column: "BlockedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_AspNetUsers_AuthorId",
                table: "Books",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlockedUsers_AspNetUsers_BlockedUserId",
                table: "BlockedUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_AspNetUsers_AuthorId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_BlockedUsers_BlockedUserId",
                table: "BlockedUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Books",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Summary",
                table: "Books",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BookCreatorUserId1",
                table: "BlockedUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlockedUsers_BookCreatorUserId1",
                table: "BlockedUsers",
                column: "BookCreatorUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_BlockedUsers_AspNetUsers_BookCreatorUserId1",
                table: "BlockedUsers",
                column: "BookCreatorUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_AspNetUsers_AuthorId",
                table: "Books",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
