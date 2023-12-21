using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobileApp.Host.Migrations
{
    public partial class EventsUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventUser_AspNetUsers_UsersAssignedId",
                table: "EventUser");

            migrationBuilder.DropForeignKey(
                name: "FK_EventUser_Events_EventsAssignedId",
                table: "EventUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventUser",
                table: "EventUser");

            migrationBuilder.DropIndex(
                name: "IX_EventUser_UsersAssignedId",
                table: "EventUser");

            migrationBuilder.RenameTable(
                name: "EventUser",
                newName: "EventUserCooperated");

            migrationBuilder.RenameColumn(
                name: "UsersAssignedId",
                table: "EventUserCooperated",
                newName: "CooperatorsId");

            migrationBuilder.RenameColumn(
                name: "EventsAssignedId",
                table: "EventUserCooperated",
                newName: "EventsCooperatedId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventUserCooperated",
                table: "EventUserCooperated",
                columns: new[] { "CooperatorsId", "EventsCooperatedId" });

            migrationBuilder.CreateTable(
                name: "EventUserAssigned",
                columns: table => new
                {
                    EventsAssignedId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsersAssignedId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventUserAssigned", x => new { x.EventsAssignedId, x.UsersAssignedId });
                    table.ForeignKey(
                        name: "FK_EventUserAssigned_AspNetUsers_UsersAssignedId",
                        column: x => x.UsersAssignedId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventUserAssigned_Events_EventsAssignedId",
                        column: x => x.EventsAssignedId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventUserPending",
                columns: table => new
                {
                    EventsPendingId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsersPendingId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventUserPending", x => new { x.EventsPendingId, x.UsersPendingId });
                    table.ForeignKey(
                        name: "FK_EventUserPending_AspNetUsers_UsersPendingId",
                        column: x => x.UsersPendingId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventUserPending_Events_EventsPendingId",
                        column: x => x.EventsPendingId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventUserCooperated_EventsCooperatedId",
                table: "EventUserCooperated",
                column: "EventsCooperatedId");

            migrationBuilder.CreateIndex(
                name: "IX_EventUserAssigned_UsersAssignedId",
                table: "EventUserAssigned",
                column: "UsersAssignedId");

            migrationBuilder.CreateIndex(
                name: "IX_EventUserPending_UsersPendingId",
                table: "EventUserPending",
                column: "UsersPendingId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventUserCooperated_AspNetUsers_CooperatorsId",
                table: "EventUserCooperated",
                column: "CooperatorsId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventUserCooperated_Events_EventsCooperatedId",
                table: "EventUserCooperated",
                column: "EventsCooperatedId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventUserCooperated_AspNetUsers_CooperatorsId",
                table: "EventUserCooperated");

            migrationBuilder.DropForeignKey(
                name: "FK_EventUserCooperated_Events_EventsCooperatedId",
                table: "EventUserCooperated");

            migrationBuilder.DropTable(
                name: "EventUserAssigned");

            migrationBuilder.DropTable(
                name: "EventUserPending");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventUserCooperated",
                table: "EventUserCooperated");

            migrationBuilder.DropIndex(
                name: "IX_EventUserCooperated_EventsCooperatedId",
                table: "EventUserCooperated");

            migrationBuilder.RenameTable(
                name: "EventUserCooperated",
                newName: "EventUser");

            migrationBuilder.RenameColumn(
                name: "EventsCooperatedId",
                table: "EventUser",
                newName: "EventsAssignedId");

            migrationBuilder.RenameColumn(
                name: "CooperatorsId",
                table: "EventUser",
                newName: "UsersAssignedId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventUser",
                table: "EventUser",
                columns: new[] { "EventsAssignedId", "UsersAssignedId" });

            migrationBuilder.CreateIndex(
                name: "IX_EventUser_UsersAssignedId",
                table: "EventUser",
                column: "UsersAssignedId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventUser_AspNetUsers_UsersAssignedId",
                table: "EventUser",
                column: "UsersAssignedId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventUser_Events_EventsAssignedId",
                table: "EventUser",
                column: "EventsAssignedId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
