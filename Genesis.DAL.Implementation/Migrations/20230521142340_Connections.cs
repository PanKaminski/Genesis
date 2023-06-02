using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Genesis.DAL.Implementation.Migrations
{
    public partial class Connections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "account_connections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountFromId = table.Column<int>(type: "int", nullable: false),
                    AccountToId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_connections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_account_connections_accounts_AccountFromId",
                        column: x => x.AccountFromId,
                        principalTable: "accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_account_connections_accounts_AccountToId",
                        column: x => x.AccountToId,
                        principalTable: "accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_account_connections_AccountFromId",
                table: "account_connections",
                column: "AccountFromId");

            migrationBuilder.CreateIndex(
                name: "IX_account_connections_AccountToId",
                table: "account_connections",
                column: "AccountToId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account_connections");
        }
    }
}
