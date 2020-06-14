using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class AuthorInfomation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AcademicRank",
                table: "Authors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Degree",
                table: "Authors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Deparment",
                table: "Authors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Specialized",
                table: "Authors",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcademicRank",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "Degree",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "Deparment",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "Specialized",
                table: "Authors");
        }
    }
}
