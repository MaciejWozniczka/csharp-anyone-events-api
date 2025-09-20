using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnyOneApi.Host.Migrations
{
    public partial class UserBirhdayYear : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Age",
                table: "AspNetUsers",
                newName: "BirthdayYear");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BirthdayYear",
                table: "AspNetUsers",
                newName: "Age");
        }
    }
}
