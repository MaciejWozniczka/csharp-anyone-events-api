using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobileApp.Host.Migrations
{
    public partial class CooperatorsPending : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventUserCooperationPending",
                columns: table => new
                {
                    CooperatorsPendingId = table.Column<string>(type: "text", nullable: false),
                    EventsCooperationPendingId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventUserCooperationPending", x => new { x.CooperatorsPendingId, x.EventsCooperationPendingId });
                    table.ForeignKey(
                        name: "FK_EventUserCooperationPending_AspNetUsers_CooperatorsPendingId",
                        column: x => x.CooperatorsPendingId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventUserCooperationPending_Events_EventsCooperationPending~",
                        column: x => x.EventsCooperationPendingId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventUserCooperationPending_EventsCooperationPendingId",
                table: "EventUserCooperationPending",
                column: "EventsCooperationPendingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventUserCooperationPending");
        }
    }
}
