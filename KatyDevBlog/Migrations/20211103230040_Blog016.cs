using Microsoft.EntityFrameworkCore.Migrations;

namespace KatyDevBlog.Migrations
{
    public partial class Blog016 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlogUserId",
                table: "BlogPosts",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_BlogUserId",
                table: "BlogPosts",
                column: "BlogUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_AspNetUsers_BlogUserId",
                table: "BlogPosts",
                column: "BlogUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_AspNetUsers_BlogUserId",
                table: "BlogPosts");

            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_BlogUserId",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "BlogUserId",
                table: "BlogPosts");
        }
    }
}
