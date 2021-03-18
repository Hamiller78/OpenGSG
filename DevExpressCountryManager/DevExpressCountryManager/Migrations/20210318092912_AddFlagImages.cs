using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DevExpressCountryManager.Migrations
{
    public partial class AddFlagImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FlagImagePath",
                table: "Countries",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BlobbableImages",
                columns: table => new
                {
                    ImagePath = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ImageData = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlobbableImages", x => x.ImagePath);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Countries_BlobbableImages_FlagImagePath",
                table: "Countries");

            migrationBuilder.DropTable(
                name: "BlobbableImages");

            migrationBuilder.DropIndex(
                name: "IX_Countries_FlagImagePath",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "FlagImagePath",
                table: "Countries");
        }
    }
}
