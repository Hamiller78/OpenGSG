using Microsoft.EntityFrameworkCore.Migrations;

namespace DevExpressCountryManager.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Tag = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    LongName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Government = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Allegiance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Leader = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Tag);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
