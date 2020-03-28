using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class AddWareHouseInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Symbol",
                table: "Warehouses",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Warehouses",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Warehouses",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Warehouses",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WarehouseType",
                table: "Warehouses",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_CategoryId",
                table: "Warehouses",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouses_Categories_CategoryId",
                table: "Warehouses",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Warehouses_Categories_CategoryId",
                table: "Warehouses");

            migrationBuilder.DropIndex(
                name: "IX_Warehouses_CategoryId",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "WarehouseType",
                table: "Warehouses");

            migrationBuilder.AlterColumn<string>(
                name: "Symbol",
                table: "Warehouses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Warehouses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
