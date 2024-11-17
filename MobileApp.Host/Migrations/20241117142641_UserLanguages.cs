using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobileApp.Host.Migrations
{
    public partial class UserLanguages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PendingUser");

            migrationBuilder.RenameColumn(
                name: "Country",
                table: "AspNetUsers",
                newName: "UserId");

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

            migrationBuilder.AddColumn<List<string>>(
                name: "Languages",
                table: "AspNetUsers",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserGroupId",
                table: "AspNetUsers",
                type: "uuid",
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

        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "Languages",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserGroupId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AspNetUsers",
                newName: "Country");

            migrationBuilder.CreateTable(
                name: "PendingUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Accepted = table.Column<bool>(type: "boolean", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletingDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    UserGroupId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
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
    }
}
