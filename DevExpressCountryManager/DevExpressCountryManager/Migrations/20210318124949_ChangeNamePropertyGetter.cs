using Microsoft.EntityFrameworkCore.Migrations;

namespace DevExpressCountryManager.Migrations
{
    public partial class ChangeNamePropertyGetter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Countries",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Countries");
        }
    }
}
