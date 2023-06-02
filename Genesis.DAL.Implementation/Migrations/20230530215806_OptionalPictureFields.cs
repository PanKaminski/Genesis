using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Genesis.DAL.Implementation.Migrations
{
    public partial class OptionalPictureFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_pictures_GenealogicalTreeId",
                table: "pictures");

            migrationBuilder.AlterColumn<int>(
                name: "PersonId",
                table: "pictures",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "HistoricalNotationId",
                table: "pictures",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "GenealogicalTreeId",
                table: "pictures",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_pictures_GenealogicalTreeId",
                table: "pictures",
                column: "GenealogicalTreeId",
                unique: true,
                filter: "[GenealogicalTreeId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_pictures_GenealogicalTreeId",
                table: "pictures");

            migrationBuilder.AlterColumn<int>(
                name: "PersonId",
                table: "pictures",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "HistoricalNotationId",
                table: "pictures",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GenealogicalTreeId",
                table: "pictures",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_pictures_GenealogicalTreeId",
                table: "pictures",
                column: "GenealogicalTreeId",
                unique: true);
        }
    }
}
