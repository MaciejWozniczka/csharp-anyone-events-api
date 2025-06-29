using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobileApp.Host.Migrations
{
    /// <inheritdoc />
    public partial class Chat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EventId",
                table: "Chats",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoomId",
                table: "Chats",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_EventId",
                table: "Chats",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Events_EventId",
                table: "Chats",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Events_EventId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_EventId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Chats");
        }
    }
}
