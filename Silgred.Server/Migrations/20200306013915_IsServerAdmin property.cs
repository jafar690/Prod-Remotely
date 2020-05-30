using Microsoft.EntityFrameworkCore.Migrations;

namespace Silgred.Server.Migrations
{
    public partial class IsServerAdminproperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                "IsServerAdmin",
                "RemotelyUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "IsServerAdmin",
                "RemotelyUsers");
        }
    }
}