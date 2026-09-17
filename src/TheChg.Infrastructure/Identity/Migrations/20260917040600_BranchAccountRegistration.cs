using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheChg.Infrastructure.Identity.Migrations
{
    /// <inheritdoc />
    public partial class BranchAccountRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("IF EXISTS (SELECT 1 FROM [identity].[AccountInvitations]) " +
                "THROW 51001, 'Existing invitation records require explicit review before retirement.', 1;");

            migrationBuilder.DropTable(
                name: "AccountInvitations",
                schema: "identity");

            migrationBuilder.CreateTable(
                name: "BranchAccountRegistrations",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegistrarId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChurchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Kind = table.Column<int>(type: "int", nullable: false),
                    CapturedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchAccountRegistrations", x => x.Id);
                    table.CheckConstraint("CK_BranchAccountRegistration_Kind", "[Kind] IN (1, 2)");
                    table.ForeignKey(
                        name: "FK_BranchAccountRegistrations_Churches_ChurchId",
                        column: x => x.ChurchId,
                        principalSchema: "organization",
                        principalTable: "Churches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BranchAccountRegistrations_OrganizationalUnits_ChurchId_BranchId",
                        columns: x => new { x.ChurchId, x.BranchId },
                        principalSchema: "organization",
                        principalTable: "OrganizationalUnits",
                        principalColumns: new[] { "ChurchId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BranchAccountRegistrations_Users_ChurchId_AccountId",
                        columns: x => new { x.ChurchId, x.AccountId },
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumns: new[] { "ChurchId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BranchAccountRegistrations_Users_ChurchId_RegistrarId",
                        columns: x => new { x.ChurchId, x.RegistrarId },
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumns: new[] { "ChurchId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BranchAccountRegistrations_AccountId",
                schema: "identity",
                table: "BranchAccountRegistrations",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BranchAccountRegistrations_ChurchId_AccountId",
                schema: "identity",
                table: "BranchAccountRegistrations",
                columns: new[] { "ChurchId", "AccountId" });

            migrationBuilder.CreateIndex(
                name: "IX_BranchAccountRegistrations_ChurchId_BranchId",
                schema: "identity",
                table: "BranchAccountRegistrations",
                columns: new[] { "ChurchId", "BranchId" });

            migrationBuilder.CreateIndex(
                name: "IX_BranchAccountRegistrations_ChurchId_RegistrarId",
                schema: "identity",
                table: "BranchAccountRegistrations",
                columns: new[] { "ChurchId", "RegistrarId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("IF EXISTS (SELECT 1 FROM [identity].[BranchAccountRegistrations]) " +
                "THROW 51002, 'Existing branch registration records require explicit review before rollback.', 1;");

            migrationBuilder.DropTable(
                name: "BranchAccountRegistrations",
                schema: "identity");

            migrationBuilder.CreateTable(
                name: "AccountInvitations",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChurchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConsumedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NormalizedDestination = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountInvitations", x => x.Id);
                    table.CheckConstraint("CK_AccountInvitation_Expiry", "[ExpiresAt] > '2000-01-01'");
                    table.ForeignKey(
                        name: "FK_AccountInvitations_Churches_ChurchId",
                        column: x => x.ChurchId,
                        principalSchema: "organization",
                        principalTable: "Churches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountInvitations_Users_ChurchId_UserId",
                        columns: x => new { x.ChurchId, x.UserId },
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumns: new[] { "ChurchId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvitations_ChurchId_UserId",
                schema: "identity",
                table: "AccountInvitations",
                columns: new[] { "ChurchId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvitations_TokenHash",
                schema: "identity",
                table: "AccountInvitations",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountInvitations_UserId_ConsumedAt",
                schema: "identity",
                table: "AccountInvitations",
                columns: new[] { "UserId", "ConsumedAt" });
        }
    }
}
