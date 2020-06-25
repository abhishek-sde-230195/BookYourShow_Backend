using Microsoft.EntityFrameworkCore.Migrations;

namespace BookYourShow.Migrations.Migrations
{
    public partial class MovieSumaryAddTableMovie : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MovieSummary",
                table: "Movie",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MovieSummary",
                table: "Movie");
        }
    }
}
