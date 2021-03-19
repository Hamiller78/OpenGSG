using Microsoft.EntityFrameworkCore.Migrations;

namespace DevExpressCountryManager.Migrations
{
    public partial class FlagsError : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Countries_BlobbableImages_FlagId",
                table: "Countries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlobbableImages",
                table: "BlobbableImages");

            migrationBuilder.RenameTable(
                name: "BlobbableImages",
                newName: "Flags");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Flags",
                table: "Flags",
                column: "BlobbableImageID");

            migrationBuilder.AddForeignKey(
                name: "FK_Countries_Flags_FlagId",
                table: "Countries",
                column: "FlagId",
                principalTable: "Flags",
                principalColumn: "BlobbableImageID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Countries_Flags_FlagId",
                table: "Countries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Flags",
                table: "Flags");

            migrationBuilder.RenameTable(
                name: "Flags",
                newName: "BlobbableImages");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlobbableImages",
                table: "BlobbableImages",
                column: "BlobbableImageID");

            migrationBuilder.AddForeignKey(
                name: "FK_Countries_BlobbableImages_FlagId",
                table: "Countries",
                column: "FlagId",
                principalTable: "BlobbableImages",
                principalColumn: "BlobbableImageID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
