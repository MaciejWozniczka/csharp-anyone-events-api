using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobileApp.Host.Migrations
{
    public partial class UserGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventUserInterested",
                columns: table => new
                {
                    EventsInterestedId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsersInterestedId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventUserInterested", x => new { x.EventsInterestedId, x.UsersInterestedId });
                    table.ForeignKey(
                        name: "FK_EventUserInterested_AspNetUsers_UsersInterestedId",
                        column: x => x.UsersInterestedId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventUserInterested_Events_EventsInterestedId",
                        column: x => x.EventsInterestedId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventUserSkipped",
                columns: table => new
                {
                    EventsSkippedId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsersSkippedId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventUserSkipped", x => new { x.EventsSkippedId, x.UsersSkippedId });
                    table.ForeignKey(
                        name: "FK_EventUserSkipped_AspNetUsers_UsersSkippedId",
                        column: x => x.UsersSkippedId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventUserSkipped_Events_EventsSkippedId",
                        column: x => x.EventsSkippedId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserGroup",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShortText = table.Column<string>(type: "text", nullable: false),
                    IsVisible = table.Column<bool>(type: "boolean", nullable: false),
                    UserEventId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletingDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserGroup_Events_UserEventId",
                        column: x => x.UserEventId,
                        principalTable: "Events",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PendingUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Accepted = table.Column<bool>(type: "boolean", nullable: false),
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
                name: "IX_EventUserInterested_UsersInterestedId",
                table: "EventUserInterested",
                column: "UsersInterestedId");

            migrationBuilder.CreateIndex(
                name: "IX_EventUserSkipped_UsersSkippedId",
                table: "EventUserSkipped",
                column: "UsersSkippedId");

            migrationBuilder.CreateIndex(
                name: "IX_PendingUser_UserGroupId",
                table: "PendingUser",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroup_UserEventId",
                table: "UserGroup",
                column: "UserEventId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventUserInterested");

            migrationBuilder.DropTable(
                name: "EventUserSkipped");

            migrationBuilder.DropTable(
                name: "PendingUser");

            migrationBuilder.DropTable(
                name: "UserGroup");
        }
    }
}
