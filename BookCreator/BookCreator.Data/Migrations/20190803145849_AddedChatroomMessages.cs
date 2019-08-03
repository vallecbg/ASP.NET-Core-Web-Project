using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookCreator.Data.Migrations
{
    public partial class AddedChatroomMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatRoomMessages",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    PublishedOn = table.Column<DateTime>(nullable: false),
                    Username = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRoomMessages", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatRoomMessages");
        }
    }
}
