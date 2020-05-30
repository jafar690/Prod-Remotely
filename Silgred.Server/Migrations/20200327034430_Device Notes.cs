using Microsoft.EntityFrameworkCore.Migrations;

namespace Silgred.Server.Migrations
{
    public partial class DeviceNotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "Notes",
                "Devices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "Notes",
                "Devices");
        }
    }
}