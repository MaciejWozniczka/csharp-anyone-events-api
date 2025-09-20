using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnyOneApi.Host.Migrations
{
    public partial class UserGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserGroup_UserGroupId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserGroupId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Accepted",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserGroupId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "PendingUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Accepted = table.Column<bool>(type: "boolean", nullable: true),
                    UserGroupId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletingDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PendingUser_UserGroup_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "UserGroup",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PendingUser_UserGroupId",
                table: "PendingUser",
                column: "UserGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PendingUser");

            migrationBuilder.AddColumn<bool>(
                name: "Accepted",
                table: "AspNetUsers",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "UserGroupId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserGroupId",
                table: "AspNetUsers",
                column: "UserGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserGroup_UserGroupId",
                table: "AspNetUsers",
                column: "UserGroupId",
                principalTable: "UserGroup",
                principalColumn: "Id");
        }
    }
}
