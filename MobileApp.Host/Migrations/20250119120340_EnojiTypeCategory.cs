using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnyOneApi.Host.Migrations
{
    public partial class EnojiTypeCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "EventTypes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Categories");

            migrationBuilder.AddColumn<string>(
                name: "EmojiCode",
                table: "Categories",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmojiCode",
                table: "Categories");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "EventTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Categories",
                type: "text",
                nullable: true);
        }
    }
}
