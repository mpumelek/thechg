using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheChg.Infrastructure.Identity.Migrations
{
    /// <inheritdoc />
    public partial class InvitationAndChurchForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Users_ChurchId_Id",
                schema: "identity",
                table: "Users",
                columns: new[] { "ChurchId", "Id" });

            migrationBuilder.CreateTable(
                name: "AccountInvitations",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChurchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NormalizedDestination = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConsumedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
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

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Churches_ChurchId",
                schema: "identity",
                table: "Users",
                column: "ChurchId",
                principalSchema: "organization",
                principalTable: "Churches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Churches_ChurchId",
                schema: "identity",
                table: "Users");

            migrationBuilder.DropTable(
                name: "AccountInvitations",
                schema: "identity");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Users_ChurchId_Id",
                schema: "identity",
                table: "Users");
        }
    }
}
