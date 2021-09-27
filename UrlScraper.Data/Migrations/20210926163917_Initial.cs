using Microsoft.EntityFrameworkCore.Migrations;

namespace UrlScraper.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScrapeRequests",
                columns: table => new
                {
                    ScrapeRequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Processed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrapeRequests", x => x.ScrapeRequestId);
                });

            migrationBuilder.CreateTable(
                name: "ScrapeRequestResults",
                columns: table => new
                {
                    ScrapeResultId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScrapeRequestId = table.Column<int>(type: "int", nullable: false),
                    ResultData = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrapeRequestResults", x => x.ScrapeResultId);
                    table.ForeignKey(
                        name: "FK_ScrapeRequestResults_ScrapeRequests_ScrapeRequestId",
                        column: x => x.ScrapeRequestId,
                        principalTable: "ScrapeRequests",
                        principalColumn: "ScrapeRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScrapeRequestResults_ScrapeRequestId",
                table: "ScrapeRequestResults",
                column: "ScrapeRequestId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScrapeRequestResults");

            migrationBuilder.DropTable(
                name: "ScrapeRequests");
        }
    }
}
