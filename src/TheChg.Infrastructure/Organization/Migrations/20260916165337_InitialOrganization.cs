using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheChg.Infrastructure.Organization.Migrations
{
    /// <inheritdoc />
    public partial class InitialOrganization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "organization");

            migrationBuilder.CreateTable(
                name: "Churches",
                schema: "organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Churches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationalUnits",
                schema: "organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChurchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnitType = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    TimeZoneId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationalUnits", x => x.Id);
                    table.UniqueConstraint("AK_OrganizationalUnits_ChurchId_Id", x => new { x.ChurchId, x.Id });
                    table.CheckConstraint("CK_OrganizationalUnit_ParentShape", "([UnitType] = 1 AND [ParentId] IS NULL) OR ([UnitType] IN (2, 3) AND [ParentId] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_OrganizationalUnits_Churches_ChurchId",
                        column: x => x.ChurchId,
                        principalSchema: "organization",
                        principalTable: "Churches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganizationalUnits_OrganizationalUnits_ChurchId_ParentId",
                        columns: x => new { x.ChurchId, x.ParentId },
                        principalSchema: "organization",
                        principalTable: "OrganizationalUnits",
                        principalColumns: new[] { "ChurchId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationalUnits_ChurchId_Code",
                schema: "organization",
                table: "OrganizationalUnits",
                columns: new[] { "ChurchId", "Code" },
                unique: true,
                filter: "[UnitType] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationalUnits_ChurchId_ParentId_Code",
                schema: "organization",
                table: "OrganizationalUnits",
                columns: new[] { "ChurchId", "ParentId", "Code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationalUnits",
                schema: "organization");

            migrationBuilder.DropTable(
                name: "Churches",
                schema: "organization");
        }
    }
}
