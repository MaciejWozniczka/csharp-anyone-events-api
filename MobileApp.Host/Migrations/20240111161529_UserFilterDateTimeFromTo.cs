using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobileApp.Host.Migrations
{
    public partial class UserFilterDateTimeFromTo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFilters_Locations_LocationId",
                table: "UserFilters");

            migrationBuilder.DropColumn(
                name: "DateFrom",
                table: "UserFilters");

            migrationBuilder.DropColumn(
                name: "DateTo",
                table: "UserFilters");

            migrationBuilder.RenameColumn(
                name: "TimeTo",
                table: "UserFilters",
                newName: "DateTimeTo");

            migrationBuilder.RenameColumn(
                name: "TimeFrom",
                table: "UserFilters",
                newName: "DateTimeFrom");

            migrationBuilder.AlterColumn<Guid>(
                name: "LocationId",
                table: "UserFilters",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFilters_Locations_LocationId",
                table: "UserFilters",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFilters_Locations_LocationId",
                table: "UserFilters");

            migrationBuilder.RenameColumn(
                name: "DateTimeTo",
                table: "UserFilters",
                newName: "TimeTo");

            migrationBuilder.RenameColumn(
                name: "DateTimeFrom",
                table: "UserFilters",
                newName: "TimeFrom");

            migrationBuilder.AlterColumn<Guid>(
                name: "LocationId",
                table: "UserFilters",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateFrom",
                table: "UserFilters",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTo",
                table: "UserFilters",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFilters_Locations_LocationId",
                table: "UserFilters",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
