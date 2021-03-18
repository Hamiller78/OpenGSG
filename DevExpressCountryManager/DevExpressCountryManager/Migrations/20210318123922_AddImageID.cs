using Microsoft.EntityFrameworkCore.Migrations;

namespace DevExpressCountryManager.Migrations
{
    public partial class AddImageID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Countries_BlobbableImages_FlagImagePath",
                table: "Countries");

            migrationBuilder.DropIndex(
                name: "IX_Countries_FlagImagePath",
                table: "Countries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlobbableImages",
                table: "BlobbableImages");

            migrationBuilder.DropColumn(
                name: "FlagImagePath",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "BlobbableImages");

            migrationBuilder.AddColumn<int>(
                name: "FlagId",
                table: "Countries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BlobbableImageID",
                table: "BlobbableImages",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlobbableImages",
                table: "BlobbableImages",
                column: "BlobbableImageID");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_FlagId",
                table: "Countries",
                column: "FlagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Countries_BlobbableImages_FlagId",
                table: "Countries",
                column: "FlagId",
                principalTable: "BlobbableImages",
                principalColumn: "BlobbableImageID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Countries_BlobbableImages_FlagId",
                table: "Countries");

            migrationBuilder.DropIndex(
                name: "IX_Countries_FlagId",
                table: "Countries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlobbableImages",
                table: "BlobbableImages");

            migrationBuilder.DropColumn(
                name: "FlagId",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "BlobbableImageID",
                table: "BlobbableImages");

            migrationBuilder.AddColumn<string>(
                name: "FlagImagePath",
                table: "Countries",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "BlobbableImages",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlobbableImages",
                table: "BlobbableImages",
                column: "ImagePath");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_FlagImagePath",
                table: "Countries",
                column: "FlagImagePath");

            migrationBuilder.AddForeignKey(
                name: "FK_Countries_BlobbableImages_FlagImagePath",
                table: "Countries",
                column: "FlagImagePath",
                principalTable: "BlobbableImages",
                principalColumn: "ImagePath",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
