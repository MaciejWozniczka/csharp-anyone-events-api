using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobileApp.Host.Migrations
{
    public partial class Location : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Addresses_LocationId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "City",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<Guid>(
                name: "AddressId",
                table: "Events",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_AddressId",
                table: "Events",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LocationId",
                table: "AspNetUsers",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Location_LocationId",
                table: "AspNetUsers",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Addresses_AddressId",
                table: "Events",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Location_LocationId",
                table: "Events",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Location_LocationId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Addresses_AddressId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Location_LocationId",
                table: "Events");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropIndex(
                name: "IX_Events_AddressId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LocationId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Addresses_LocationId",
                table: "Events",
                column: "LocationId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
