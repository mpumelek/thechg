using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheChg.Infrastructure.Authorization.Migrations
{
    /// <inheritdoc />
    public partial class InitialPermissionGrants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "authorization");

            migrationBuilder.CreateTable(
                name: "PermissionGrants",
                schema: "authorization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChurchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Permission = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ScopeUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IncludeDescendants = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EffectiveUntil = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RevokedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionGrants", x => x.Id);
                    table.CheckConstraint("CK_PermissionGrant_EffectiveRange", "[EffectiveUntil] IS NULL OR [EffectiveUntil] > [EffectiveFrom]");
                    table.CheckConstraint("CK_PermissionGrant_NonemptyIds", "[Id] <> '00000000-0000-0000-0000-000000000000' AND [AccountId] <> '00000000-0000-0000-0000-000000000000' AND [ChurchId] <> '00000000-0000-0000-0000-000000000000' AND ([ScopeUnitId] IS NULL OR [ScopeUnitId] <> '00000000-0000-0000-0000-000000000000')");
                    table.CheckConstraint("CK_PermissionGrant_Permission", "TRIM([Permission]) <> '' AND [Permission] = TRIM([Permission])");
                    table.CheckConstraint("CK_PermissionGrant_ScopeShape", "[ScopeUnitId] IS NOT NULL OR [IncludeDescendants] = 0");
                    table.ForeignKey(
                        name: "FK_PermissionGrants_Churches_ChurchId",
                        column: x => x.ChurchId,
                        principalSchema: "organization",
                        principalTable: "Churches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PermissionGrants_OrganizationalUnits_ChurchId_ScopeUnitId",
                        columns: x => new { x.ChurchId, x.ScopeUnitId },
                        principalSchema: "organization",
                        principalTable: "OrganizationalUnits",
                        principalColumns: new[] { "ChurchId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PermissionGrants_Users_ChurchId_AccountId",
                        columns: x => new { x.ChurchId, x.AccountId },
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumns: new[] { "ChurchId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PermissionGrants_AccountId_ChurchId_Permission",
                schema: "authorization",
                table: "PermissionGrants",
                columns: new[] { "AccountId", "ChurchId", "Permission" });

            migrationBuilder.CreateIndex(
                name: "IX_PermissionGrants_AccountId_ChurchId_Permission_ScopeUnitId_EffectiveFrom",
                schema: "authorization",
                table: "PermissionGrants",
                columns: new[] { "AccountId", "ChurchId", "Permission", "ScopeUnitId", "EffectiveFrom" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermissionGrants_ChurchId_AccountId",
                schema: "authorization",
                table: "PermissionGrants",
                columns: new[] { "ChurchId", "AccountId" });

            migrationBuilder.CreateIndex(
                name: "IX_PermissionGrants_ChurchId_ScopeUnitId",
                schema: "authorization",
                table: "PermissionGrants",
                columns: new[] { "ChurchId", "ScopeUnitId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PermissionGrants",
                schema: "authorization");
        }
    }
}
