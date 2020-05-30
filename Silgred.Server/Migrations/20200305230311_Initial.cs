using Microsoft.EntityFrameworkCore.Migrations;

namespace Silgred.Server.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "AspNetRoles",
                table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_AspNetRoles", x => x.Id); });

            migrationBuilder.CreateTable(
                "Organizations",
                table => new
                {
                    ID = table.Column<string>(nullable: false),
                    OrganizationName = table.Column<string>(maxLength: 25, nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Organizations", x => x.ID); });

            migrationBuilder.CreateTable(
                "AspNetRoleClaims",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        x => x.RoleId,
                        "AspNetRoles",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "ApiTokens",
                table => new
                {
                    ID = table.Column<string>(nullable: false),
                    LastUsed = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    OrganizationID = table.Column<string>(nullable: true),
                    Secret = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiTokens", x => x.ID);
                    table.ForeignKey(
                        "FK_ApiTokens_Organizations_OrganizationID",
                        x => x.OrganizationID,
                        "Organizations",
                        "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "CommandResults",
                table => new
                {
                    ID = table.Column<string>(nullable: false),
                    CommandMode = table.Column<string>(nullable: true),
                    CommandText = table.Column<string>(nullable: true),
                    SenderUserID = table.Column<string>(nullable: true),
                    SenderConnectionID = table.Column<string>(nullable: true),
                    TargetDeviceIDs = table.Column<string>(nullable: true),
                    PSCoreResults = table.Column<string>(nullable: true),
                    CommandResults = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<string>(nullable: false),
                    OrganizationID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommandResults", x => x.ID);
                    table.ForeignKey(
                        "FK_CommandResults_Organizations_OrganizationID",
                        x => x.OrganizationID,
                        "Organizations",
                        "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "DeviceGroups",
                table => new
                {
                    ID = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    OrganizationID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceGroups", x => x.ID);
                    table.ForeignKey(
                        "FK_DeviceGroups_Organizations_OrganizationID",
                        x => x.OrganizationID,
                        "Organizations",
                        "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "EventLogs",
                table => new
                {
                    ID = table.Column<string>(nullable: false),
                    EventType = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    Source = table.Column<string>(nullable: true),
                    StackTrace = table.Column<string>(nullable: true),
                    OrganizationID = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventLogs", x => x.ID);
                    table.ForeignKey(
                        "FK_EventLogs_Organizations_OrganizationID",
                        x => x.OrganizationID,
                        "Organizations",
                        "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "InviteLinks",
                table => new
                {
                    ID = table.Column<string>(nullable: false),
                    InvitedUser = table.Column<string>(nullable: true),
                    IsAdmin = table.Column<bool>(nullable: false),
                    DateSent = table.Column<string>(nullable: false),
                    OrganizationID = table.Column<string>(nullable: true),
                    ResetUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InviteLinks", x => x.ID);
                    table.ForeignKey(
                        "FK_InviteLinks_Organizations_OrganizationID",
                        x => x.OrganizationID,
                        "Organizations",
                        "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "RemotelyUsers",
                table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<string>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    UserOptions = table.Column<string>(nullable: true),
                    OrganizationID = table.Column<string>(nullable: true),
                    IsAdministrator = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemotelyUsers", x => x.Id);
                    table.ForeignKey(
                        "FK_RemotelyUsers_Organizations_OrganizationID",
                        x => x.OrganizationID,
                        "Organizations",
                        "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "SharedFiles",
                table => new
                {
                    ID = table.Column<string>(nullable: false),
                    FileName = table.Column<string>(nullable: true),
                    ContentType = table.Column<string>(nullable: true),
                    FileContents = table.Column<byte[]>(nullable: true),
                    Timestamp = table.Column<string>(nullable: false),
                    OrganizationID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedFiles", x => x.ID);
                    table.ForeignKey(
                        "FK_SharedFiles_Organizations_OrganizationID",
                        x => x.OrganizationID,
                        "Organizations",
                        "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "Devices",
                table => new
                {
                    ID = table.Column<string>(nullable: false),
                    AgentVersion = table.Column<string>(nullable: true),
                    Alias = table.Column<string>(maxLength: 100, nullable: true),
                    CpuUtilization = table.Column<double>(nullable: false),
                    CurrentUser = table.Column<string>(nullable: true),
                    DeviceGroupID = table.Column<string>(nullable: true),
                    DeviceName = table.Column<string>(nullable: true),
                    Drives = table.Column<string>(nullable: true),
                    UsedMemory = table.Column<double>(nullable: false),
                    UsedStorage = table.Column<double>(nullable: false),
                    Is64Bit = table.Column<bool>(nullable: false),
                    IsOnline = table.Column<bool>(nullable: false),
                    LastOnline = table.Column<string>(nullable: false),
                    OrganizationID = table.Column<string>(nullable: true),
                    OSArchitecture = table.Column<int>(nullable: false),
                    OSDescription = table.Column<string>(nullable: true),
                    Platform = table.Column<string>(nullable: true),
                    ProcessorCount = table.Column<int>(nullable: false),
                    ServerVerificationToken = table.Column<string>(nullable: true),
                    Tags = table.Column<string>(maxLength: 200, nullable: true),
                    TotalMemory = table.Column<double>(nullable: false),
                    TotalStorage = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.ID);
                    table.ForeignKey(
                        "FK_Devices_DeviceGroups_DeviceGroupID",
                        x => x.DeviceGroupID,
                        "DeviceGroups",
                        "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Devices_Organizations_OrganizationID",
                        x => x.OrganizationID,
                        "Organizations",
                        "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "AspNetUserClaims",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        "FK_AspNetUserClaims_RemotelyUsers_UserId",
                        x => x.UserId,
                        "RemotelyUsers",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AspNetUserLogins",
                table => new
                {
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new {x.LoginProvider, x.ProviderKey});
                    table.ForeignKey(
                        "FK_AspNetUserLogins_RemotelyUsers_UserId",
                        x => x.UserId,
                        "RemotelyUsers",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AspNetUserRoles",
                table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new {x.UserId, x.RoleId});
                    table.ForeignKey(
                        "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        x => x.RoleId,
                        "AspNetRoles",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_AspNetUserRoles_RemotelyUsers_UserId",
                        x => x.UserId,
                        "RemotelyUsers",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AspNetUserTokens",
                table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new {x.UserId, x.LoginProvider, x.Name});
                    table.ForeignKey(
                        "FK_AspNetUserTokens_RemotelyUsers_UserId",
                        x => x.UserId,
                        "RemotelyUsers",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "PermissionLinks",
                table => new
                {
                    ID = table.Column<string>(nullable: false),
                    UserID = table.Column<string>(nullable: true),
                    DeviceGroupID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionLinks", x => x.ID);
                    table.ForeignKey(
                        "FK_PermissionLinks_DeviceGroups_DeviceGroupID",
                        x => x.DeviceGroupID,
                        "DeviceGroups",
                        "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_PermissionLinks_RemotelyUsers_UserID",
                        x => x.UserID,
                        "RemotelyUsers",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                "IX_ApiTokens_OrganizationID",
                "ApiTokens",
                "OrganizationID");

            migrationBuilder.CreateIndex(
                "IX_ApiTokens_Token",
                "ApiTokens",
                "Token");

            migrationBuilder.CreateIndex(
                "IX_AspNetRoleClaims_RoleId",
                "AspNetRoleClaims",
                "RoleId");

            migrationBuilder.CreateIndex(
                "RoleNameIndex",
                "AspNetRoles",
                "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_AspNetUserClaims_UserId",
                "AspNetUserClaims",
                "UserId");

            migrationBuilder.CreateIndex(
                "IX_AspNetUserLogins_UserId",
                "AspNetUserLogins",
                "UserId");

            migrationBuilder.CreateIndex(
                "IX_AspNetUserRoles_RoleId",
                "AspNetUserRoles",
                "RoleId");

            migrationBuilder.CreateIndex(
                "IX_CommandResults_OrganizationID",
                "CommandResults",
                "OrganizationID");

            migrationBuilder.CreateIndex(
                "IX_DeviceGroups_OrganizationID",
                "DeviceGroups",
                "OrganizationID");

            migrationBuilder.CreateIndex(
                "IX_Devices_DeviceGroupID",
                "Devices",
                "DeviceGroupID");

            migrationBuilder.CreateIndex(
                "IX_Devices_DeviceName",
                "Devices",
                "DeviceName");

            migrationBuilder.CreateIndex(
                "IX_Devices_OrganizationID",
                "Devices",
                "OrganizationID");

            migrationBuilder.CreateIndex(
                "IX_EventLogs_OrganizationID",
                "EventLogs",
                "OrganizationID");

            migrationBuilder.CreateIndex(
                "IX_InviteLinks_OrganizationID",
                "InviteLinks",
                "OrganizationID");

            migrationBuilder.CreateIndex(
                "IX_PermissionLinks_DeviceGroupID",
                "PermissionLinks",
                "DeviceGroupID");

            migrationBuilder.CreateIndex(
                "IX_PermissionLinks_UserID",
                "PermissionLinks",
                "UserID");

            migrationBuilder.CreateIndex(
                "EmailIndex",
                "RemotelyUsers",
                "NormalizedEmail");

            migrationBuilder.CreateIndex(
                "UserNameIndex",
                "RemotelyUsers",
                "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_RemotelyUsers_OrganizationID",
                "RemotelyUsers",
                "OrganizationID");

            migrationBuilder.CreateIndex(
                "IX_RemotelyUsers_UserName",
                "RemotelyUsers",
                "UserName");

            migrationBuilder.CreateIndex(
                "IX_SharedFiles_OrganizationID",
                "SharedFiles",
                "OrganizationID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "ApiTokens");

            migrationBuilder.DropTable(
                "AspNetRoleClaims");

            migrationBuilder.DropTable(
                "AspNetUserClaims");

            migrationBuilder.DropTable(
                "AspNetUserLogins");

            migrationBuilder.DropTable(
                "AspNetUserRoles");

            migrationBuilder.DropTable(
                "AspNetUserTokens");

            migrationBuilder.DropTable(
                "CommandResults");

            migrationBuilder.DropTable(
                "Devices");

            migrationBuilder.DropTable(
                "EventLogs");

            migrationBuilder.DropTable(
                "InviteLinks");

            migrationBuilder.DropTable(
                "PermissionLinks");

            migrationBuilder.DropTable(
                "SharedFiles");

            migrationBuilder.DropTable(
                "AspNetRoles");

            migrationBuilder.DropTable(
                "DeviceGroups");

            migrationBuilder.DropTable(
                "RemotelyUsers");

            migrationBuilder.DropTable(
                "Organizations");
        }
    }
}