using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AuthMigrations.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthUsers",
                columns: table => new
                {
                    Guid = table.Column<byte[]>(nullable: false),
                    Username = table.Column<string>(maxLength: 30, nullable: false),
                    Salt = table.Column<byte[]>(maxLength: 264, nullable: false),
                    PasswordHash = table.Column<byte[]>(maxLength: 264, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthUsers", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    UserGuid = table.Column<byte[]>(nullable: false),
                    RefreshTokenString = table.Column<string>(maxLength: 512, nullable: false),
                    IssuedAt = table.Column<DateTime>(nullable: false),
                    ExpiresAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => new { x.UserGuid, x.RefreshTokenString });
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AuthUsers_UserGuid",
                        column: x => x.UserGuid,
                        principalTable: "AuthUsers",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthUsers_Username",
                table: "AuthUsers",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_ExpiresAt",
                table: "RefreshTokens",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_IssuedAt",
                table: "RefreshTokens",
                column: "IssuedAt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "AuthUsers");
        }
    }
}
