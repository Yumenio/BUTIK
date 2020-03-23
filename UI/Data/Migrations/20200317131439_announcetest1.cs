using Microsoft.EntityFrameworkCore.Migrations;

namespace UI.Data.Migrations
{
    public partial class announcetest1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Announce",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Announce");
        }
    }
}
