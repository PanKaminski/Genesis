using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Genesis.DAL.Implementation.Migrations
{
    public partial class PictureFKRestriction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pictures_genealogical_trees_GenealogicalTreeId",
                table: "pictures");

            migrationBuilder.AddForeignKey(
                name: "FK_pictures_genealogical_trees_GenealogicalTreeId",
                table: "pictures",
                column: "GenealogicalTreeId",
                principalTable: "genealogical_trees",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pictures_genealogical_trees_GenealogicalTreeId",
                table: "pictures");

            migrationBuilder.AddForeignKey(
                name: "FK_pictures_genealogical_trees_GenealogicalTreeId",
                table: "pictures",
                column: "GenealogicalTreeId",
                principalTable: "genealogical_trees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
