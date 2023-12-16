using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobileApp.Host.Migrations
{
    public partial class EventPrototype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Budget",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Cities",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Countries",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Destination",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ExperienceLevels",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Languages",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Nationalities",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "TripType",
                table: "Events");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Events",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Events",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Budget",
                table: "Events",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "Cities",
                table: "Events",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<int[]>(
                name: "Countries",
                table: "Events",
                type: "integer[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Destination",
                table: "Events",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int[]>(
                name: "ExperienceLevels",
                table: "Events",
                type: "integer[]",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "Languages",
                table: "Events",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "Nationalities",
                table: "Events",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TripType",
                table: "Events",
                type: "integer",
                nullable: true);
        }
    }
}
