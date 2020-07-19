using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AuthMigrations.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthRole",
                columns: table => new
                {
                    Id = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Username = table.Column<string>(maxLength: 30, nullable: false),
                    Salt = table.Column<byte[]>(maxLength: 264, nullable: false),
                    PasswordHash = table.Column<byte[]>(maxLength: 264, nullable: false),
                    AuthRoleId = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthUsers_AuthRole_AuthRoleId",
                        column: x => x.AuthRoleId,
                        principalTable: "AuthRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RefreshTokenString = table.Column<string>(maxLength: 512, nullable: false),
                    IssuedAt = table.Column<DateTime>(nullable: false),
                    ExpiresAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => new { x.UserId, x.RefreshTokenString });
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AuthUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AuthUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthUsers_AuthRoleId",
                table: "AuthUsers",
                column: "AuthRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthUsers_Username",
                table: "AuthUsers",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_ExpiresAt",
                table: "RefreshTokens",
                column: "ExpiresAt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "AuthUsers");

            migrationBuilder.DropTable(
                name: "AuthRole");
        }
    }
}
