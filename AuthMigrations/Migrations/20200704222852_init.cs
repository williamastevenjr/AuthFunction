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
                    AuthUserGuid = table.Column<byte[]>(nullable: false),
                    Username = table.Column<string>(maxLength: 30, nullable: false),
                    Salt = table.Column<byte[]>(maxLength: 264, nullable: false),
                    PasswordHash = table.Column<byte[]>(maxLength: 264, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthUsers", x => x.AuthUserGuid);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthUsers_Username",
                table: "AuthUsers",
                column: "Username",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthUsers");
        }
    }
}
