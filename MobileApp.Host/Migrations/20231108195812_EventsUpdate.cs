using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobileApp.Host.Migrations
{
    public partial class EventsUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TripType",
                table: "Events",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int[]>(
                name: "SexTypes",
                table: "Events",
                type: "integer[]",
                nullable: true,
                oldClrType: typeof(int[]),
                oldType: "integer[]");

            migrationBuilder.AlterColumn<List<string>>(
                name: "Nationalities",
                table: "Events",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(List<string>),
                oldType: "text[]");

            migrationBuilder.AlterColumn<List<string>>(
                name: "Languages",
                table: "Events",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(List<string>),
                oldType: "text[]");

            migrationBuilder.AlterColumn<int[]>(
                name: "ExperienceLevels",
                table: "Events",
                type: "integer[]",
                nullable: true,
                oldClrType: typeof(int[]),
                oldType: "integer[]");

            migrationBuilder.AlterColumn<string>(
                name: "Destination",
                table: "Events",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int[]>(
                name: "Countries",
                table: "Events",
                type: "integer[]",
                nullable: true,
                oldClrType: typeof(int[]),
                oldType: "integer[]");

            migrationBuilder.AlterColumn<List<string>>(
                name: "Cities",
                table: "Events",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(List<string>),
                oldType: "text[]");

            migrationBuilder.AlterColumn<string>(
                name: "Budget",
                table: "Events",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "AgeTo",
                table: "Events",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "AgeFrom",
                table: "Events",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TripType",
                table: "Events",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int[]>(
                name: "SexTypes",
                table: "Events",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0],
                oldClrType: typeof(int[]),
                oldType: "integer[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<List<string>>(
                name: "Nationalities",
                table: "Events",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(List<string>),
                oldType: "text[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<List<string>>(
                name: "Languages",
                table: "Events",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(List<string>),
                oldType: "text[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<int[]>(
                name: "ExperienceLevels",
                table: "Events",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0],
                oldClrType: typeof(int[]),
                oldType: "integer[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Destination",
                table: "Events",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int[]>(
                name: "Countries",
                table: "Events",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0],
                oldClrType: typeof(int[]),
                oldType: "integer[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<List<string>>(
                name: "Cities",
                table: "Events",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(List<string>),
                oldType: "text[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Budget",
                table: "Events",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AgeTo",
                table: "Events",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AgeFrom",
                table: "Events",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
