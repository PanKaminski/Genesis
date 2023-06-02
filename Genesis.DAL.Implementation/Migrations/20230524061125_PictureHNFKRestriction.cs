using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Genesis.DAL.Implementation.Migrations
{
    public partial class PictureHNFKRestriction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pictures_notations_HistoricalNotationId",
                table: "pictures");

            migrationBuilder.AddForeignKey(
                name: "FK_pictures_notations_HistoricalNotationId",
                table: "pictures",
                column: "HistoricalNotationId",
                principalTable: "notations",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pictures_notations_HistoricalNotationId",
                table: "pictures");

            migrationBuilder.AddForeignKey(
                name: "FK_pictures_notations_HistoricalNotationId",
                table: "pictures",
                column: "HistoricalNotationId",
                principalTable: "notations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
