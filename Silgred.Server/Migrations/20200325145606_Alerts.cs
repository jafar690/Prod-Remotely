using Microsoft.EntityFrameworkCore.Migrations;

namespace Silgred.Server.Migrations
{
    public partial class Alerts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Alerts",
                table => new
                {
                    ID = table.Column<string>(nullable: false),
                    CreatedOn = table.Column<string>(nullable: false),
                    DeviceID = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    OrganizationID = table.Column<string>(nullable: true),
                    UserID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.ID);
                    table.ForeignKey(
                        "FK_Alerts_Devices_DeviceID",
                        x => x.DeviceID,
                        "Devices",
                        "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Alerts_Organizations_OrganizationID",
                        x => x.OrganizationID,
                        "Organizations",
                        "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Alerts_RemotelyUsers_UserID",
                        x => x.UserID,
                        "RemotelyUsers",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                "IX_Alerts_DeviceID",
                "Alerts",
                "DeviceID");

            migrationBuilder.CreateIndex(
                "IX_Alerts_OrganizationID",
                "Alerts",
                "OrganizationID");

            migrationBuilder.CreateIndex(
                "IX_Alerts_UserID",
                "Alerts",
                "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Alerts");
        }
    }
}